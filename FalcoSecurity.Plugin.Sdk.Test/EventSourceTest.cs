using FalcoSecurity.Plugin.Sdk.Events;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Channels;
using Xunit;

namespace FalcoSecurity.Plugin.Sdk.Test
{
    /*
     * !!!
     TODO: There seems to be a bug in EventBatch.Dispose()
     that crashes the test host, needs investigation.
     */
    public class EventSourceTest
    {
        private static ReadOnlySpan<byte> RandomBytes(int dataSize)
        {
            var bytes = new byte[dataSize];
            Random.Shared.NextBytes(bytes);
            return bytes;
        }

        [Fact]
        public void TestEventPoolSize()
        {
            /*using*/ var eventPool = new EventBatch(size: 1, dataSize: 1);

            Assert.Equal(1, eventPool.Length);
        }

        [Fact]
        public void EventPoolGetWriteRead()
        {
            using var eventPool = new EventBatch(
                size: EventSourceConsts.DefaultBatchSize, 
                dataSize: EventSourceConsts.DefaultEventSize);

            Assert.Equal(
                EventSourceConsts.DefaultBatchSize, 
                eventPool.Length);

            for (var i = 0; i < EventSourceConsts.DefaultBatchSize; i++)
            {
                var e = eventPool.Get(i);
                e.Write(BitConverter.GetBytes(i));
            }

            unsafe
            {
                var eventsPtr = eventPool.UnderlyingArray;

                for (var i = 0; i < EventSourceConsts.DefaultBatchSize; i++)
                {
                    var evtPtr = ((PluginEvent*)eventsPtr)[i];
                    var reader = new EventReader(&evtPtr);
                    var bytes = reader.Data.ToArray();
                    var data = BitConverter.ToInt32(bytes);
                    Assert.Equal(i, data);
                }
            }
        }

        /*
        [Fact]
        public void GetOnDisposedEventBatchThrows()
        {
            var batch = new EventBatch(1, 1);
            batch.Dispose();
            Assert.Throws<ObjectDisposedException>(() =>
            {
                _ = batch.Get(0);
            });
        }       
        */

        [Fact]
        public void GetOutOfRangeOnEventBatchThrows()
        {
            /*using*/ var batch = new EventBatch(1, 1);
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                _ = batch.Get(2);
            });
        }

        [Fact]
        unsafe public void EventWriterNew()
        {
            var evt = (PluginEvent*)Marshal.AllocHGlobal(sizeof(PluginEvent));
            var ew = new EventWriter(evt, 1);
            try
            {
                Assert.Equal((nint) evt, (nint) ew.UnderlyingEvent);
                ew.Free();
                Assert.Equal(0u, evt->DataLen);
                Assert.Equal(IntPtr.Zero, evt->Data);
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr)evt);
            }
        }

        [Fact]
        unsafe public void EventWriterFree()
        {
            var evt = (PluginEvent*)Marshal.AllocHGlobal(sizeof(PluginEvent));
            var ew = new EventWriter(evt, 1);
            try
            {
                ew.Free();
                Assert.Equal(0u, evt->DataLen);
                Assert.Equal(IntPtr.Zero, evt->Data);
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr) evt);
            }
        }

        [Fact]
        unsafe public void ReadWriteArbitraryData()
        {
            var evt = (PluginEvent*)Marshal.AllocHGlobal(sizeof(PluginEvent));

            try
            {
                Assert.NotEqual(IntPtr.Zero, (IntPtr)evt);

                const int dataSize = 64;

                var ew = new EventWriter(evt, dataSize);

                var data = RandomBytes(dataSize);

                ew.Write(data);

                Assert.Equal((uint)dataSize, evt->DataLen);

                var evtData = (byte*)evt->Data;

                for (var i = 0; i < data.Length; i++)
                {
                    Assert.Equal(data[i], evtData[i]);
                }

                ew.Free();
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr) evt);
            }
        }

        [Fact]
        unsafe public void EventReaderTest()
        {
            var evt = (PluginEvent*)Marshal.AllocHGlobal(sizeof(PluginEvent));

            const string data = "This is a test!";
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var nanos = DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1000000;
            var dataBuff = Marshal.AllocHGlobal(dataBytes.Length);
            dataBytes.CopyTo(new Span<byte>((void*) dataBuff, dataBytes.Length));

            try
            {
                evt->EventNum = 42;
                evt->Timestamp = (ulong) nanos;
                evt->Data = dataBuff;
                evt->DataLen = (uint) dataBytes.Length;

                var r = new EventReader(evt);

                Assert.Equal(42u, r.EventNum);
                Assert.Equal((ulong) nanos, r.Timestamp);
                Assert.Equal(data, Encoding.UTF8.GetString(r.Data));
            }
            finally
            {
                Marshal.FreeHGlobal(evt->Data);
                Marshal.FreeHGlobal((IntPtr)evt);
            }
        }

        [Fact]
        public void PushEventSourceInstanceTest()
        {
            var instance = new TestPushEventSource(
                    EventSourceConsts.DefaultBatchSize,
                    EventSourceConsts.DefaultEventSize);

            void producer(ChannelWriter<PushEvent> channel, Action eof)
            {
                Task.Run(() =>
                {
                    while (instance.Counter < EventSourceConsts.DefaultBatchSize)
                    {
                        instance.Counter += 1;
                        channel.TryWrite(new(
                            ulong.MaxValue,
                                BitConverter.GetBytes(instance.Counter))
                            );
                    }

                    eof();
                });
            }

            instance.EventProducer = producer;

            instance.Start();
                
            EventSourceInstanceContext ctx = null;

            uint totEvents = 0;

            for (var i = 0; i < EventSourceConsts.DefaultBatchSize; i++)
            {
                ctx = instance.NextBatch();
                totEvents += ctx.BatchEventsNum;
                if (ctx.IsEof)
                {
                    break;
                }
            }

            Assert.True(ctx!.IsEof);
            Assert.Equal((uint) EventSourceConsts.DefaultBatchSize, totEvents);

            unsafe
            {
                var eventsPtr = instance.EventBatch.UnderlyingArray;

                for (var i = 0; i < EventSourceConsts.DefaultBatchSize; i++)
                {
                    var evtPtr = ((PluginEvent*)eventsPtr)[i];
                    var reader = new EventReader(&evtPtr);
                    var bytes = reader.Data.ToArray();
                    var data = BitConverter.ToInt32(bytes);
                    Assert.Equal(i + 1, data);
                }
            }
        }

        [Fact]
        public void PushEvenSourceShouldPropagateError()
        {
            var instance = new TestPushEventSource(
                    EventSourceConsts.DefaultBatchSize,
                    EventSourceConsts.DefaultEventSize);

            static void Producer(ChannelWriter<PushEvent> channel, Action eof)
            {
                channel.TryWrite(new PushEvent(
                    ulong.MaxValue,
                    ReadOnlyMemory<byte>.Empty)
                {
                    HasError = true,
                    Exception = new Exception()
                });
            }

            instance.EventProducer = Producer;

            instance.Start();

            var ctx = instance.NextBatch();

            Assert.True(ctx.HasFailure);
        }

        [Fact]
        public void PushEvenSourceTimeout()
        {
            var instance = new TestPushEventSource(
                    EventSourceConsts.DefaultBatchSize,
                    EventSourceConsts.DefaultEventSize);

            var timeout = 1;

            instance.TimeoutMs = timeout;

            void Producer(ChannelWriter<PushEvent> channel, Action eof)
            {
                Thread.Sleep(timeout * 3);
            }

            instance.EventProducer = Producer;

            instance.Start();

            var ctx = instance.NextBatch();

            Assert.True(ctx.HasTimeout);
        }

        [Fact]
        public void PullEventSourceShouldTimeout()
        {
            /*using*/ var instance = new TestPullEventSource(
                    EventSourceConsts.DefaultBatchSize,
                    EventSourceConsts.DefaultEventSize);

            var timeout = 1;

            instance.TimeoutMs = timeout;

            instance.PullEventDelegate = (ctx, evt) =>
            {
                Thread.Sleep(timeout * 3);
            };

            var ctx = instance.NextBatch();

            Assert.True(ctx.HasTimeout);
        }

        [Fact]
        public void PullEvenSourceShouldPropagateError()
        {
            /*using*/ var instance = new TestPullEventSource(
                    EventSourceConsts.DefaultBatchSize,
                    EventSourceConsts.DefaultEventSize);

            instance.PullEventDelegate = (ctx, evt) =>
            {
                throw new Exception();
            };

            var ctx = instance.NextBatch();

            Assert.True(ctx.HasFailure);
        }

        [Fact]
        public void PullEventSourceInstanceTest()
        {
            /*using*/ var instance = new TestPullEventSource(
                    EventSourceConsts.DefaultBatchSize,
                    EventSourceConsts.DefaultEventSize);

            instance.PullEventDelegate = (ctx, evt) =>
            {

                instance.Counter += 1;

                if (instance.Counter >= EventSourceConsts.DefaultBatchSize)
                {
                    ctx.IsEof = true;
                }

                evt.Write(BitConverter.GetBytes(instance.Counter));
            };

            var ctx = instance.NextBatch();

            Assert.True(ctx.IsEof);

            Assert.Equal(
                (uint) EventSourceConsts.DefaultBatchSize,
                ctx.BatchEventsNum);

            Assert.Equal(
                EventSourceConsts.DefaultBatchSize,
                instance.EventBatch.Length);

            unsafe
            {
                var eventsPtr = instance.EventBatch.UnderlyingArray;

                for (var i = 0; i < EventSourceConsts.DefaultBatchSize; i++)
                {
                    var evtPtr = ((PluginEvent*)eventsPtr)[i];
                    var reader = new EventReader(&evtPtr);
                    var bytes = reader.Data.ToArray();
                    var data = BitConverter.ToInt32(bytes);
                    Assert.Equal(i + 1, data);
                }
            }
        }
    }
}
