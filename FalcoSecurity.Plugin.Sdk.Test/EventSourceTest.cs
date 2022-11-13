using FalcoSecurity.Plugin.Sdk.Events;
using System.Runtime.InteropServices;
using System.Text;
using Xunit;

namespace FalcoSecurity.Plugin.Sdk.Test
{

    internal class TestPushEventSource : PushEventSourceInstance
    {
        public int Counter { get; private set; }

        public TestPushEventSource(int batchSize, int eventSize) : base(batchSize, eventSize)
        {
            TimeoutMs = 0;

            Task.Run(async () =>
            {
                while (Counter < EventSourceConsts.DefaultBatchSize)
                {
                    Counter += 1;

                    await EventsChannel.WriteAsync(new(
                        ulong.MaxValue,
                        BitConverter.GetBytes(Counter))
                    );
                }

                EventsChannel.Complete();
            });
        }
    }

    public unsafe class EventSourceTest
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
            using var eventPool = new EventBatch(size: 1, dataSize: 1);

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

        [Fact]
        public void ReadWriteArbitraryData()
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
            }
            finally
            {
                Marshal.FreeHGlobal((IntPtr) evt);
            }
        }

        [Fact]
        public void EventReaderTest()
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
        public void PullEventSourceInstanceTest()
        {
            using var instance = new TestPullEventSource(
                    EventSourceConsts.DefaultBatchSize,
                    EventSourceConsts.DefaultEventSize);

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
