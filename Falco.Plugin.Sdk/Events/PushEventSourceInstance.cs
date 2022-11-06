using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Threading.Channels;

namespace Falco.Plugin.Sdk.Events
{
    public readonly record struct PushEvent(
        ulong TimestampNano,
        ReadOnlyMemory<byte> Data,
        Exception? Exception,
        bool HasTimeout,
        bool HasError);

    public abstract class PushEventSourceInstance : BaseEventSourceInstance
    {
        private readonly Channel<PushEvent> _channel;

        protected readonly ChannelWriter<PushEvent> EventsChannel;

        private bool _eofReached = false;

        private Stopwatch _stopwatch;

        protected PushEventSourceInstance(
            int batchSize, 
            int eventSize,
            BoundedChannelOptions? boundedChannelOptions=null):  base(batchSize, eventSize)
        {
            var opts = boundedChannelOptions ?? new BoundedChannelOptions(batchSize)
            {
                SingleReader = true,
                SingleWriter = true,
                FullMode = BoundedChannelFullMode.DropOldest
            };

            _channel = Channel.CreateBounded<PushEvent>(opts);

            EventsChannel = _channel.Writer;

            _stopwatch = new Stopwatch();
        }

        protected void Eof(Exception? exception)
        {
            _channel.Writer.Complete(exception);
        }

        public override void Dispose()
        {
            base.Dispose();
            _channel.Writer.Complete();
            GC.SuppressFinalize(this);
        }

        override public EventSourceInstanceContext NextBatch()
        {
            if (_eofReached)
            {
                return EventSourceInstanceContext.Eof;
            }

            var ctx = new EventSourceInstanceContext();

            if (TimeoutMs > 0)
            {
                _stopwatch.Restart();
            }

            var n = 0;

            for (; n < EventBatch.Length;)
            {
                if (TimeoutMs > 0 && _stopwatch.ElapsedMilliseconds > TimeoutMs)
                {
                    // flush a partial batch
                    _stopwatch.Stop();
                    ctx.HasTimeout = true;
                    ctx.BatchEventsNum = (uint)n;
                    return ctx;
                }

                if (_channel.Reader.Completion.IsCompleted)
                {
                    // channel closed or EOF, flush a partial batch
                    _eofReached = true;
                    ctx.BatchEventsNum = (uint)n;
                    return ctx;
                }

                try
                {
                    if(_channel.Reader.TryRead(out var evt))
                    {
                        if (evt.HasError)
                        {
                            _eofReached = true;
                            ctx.HasFailure = true;
                            ctx.Error = evt.Exception?.ToString();
                            ctx.BatchEventsNum = 0;
                            return ctx;
                        }

                        var writer = EventBatch.Get(n);

                        writer.Write(evt.Data.Span);

                        writer.SetTimestamp(evt.TimestampNano == 0 
                            ? ulong.MaxValue 
                            : evt.TimestampNano);

                        _stopwatch.Restart();

                        n += 1;
                    }
                }
                catch (Exception ex)
                {
                    _eofReached = true;
                    ctx.HasFailure = true;
                    ctx.Error = ex.ToString();
                    ctx.BatchEventsNum = 0;
                    return ctx;
                }
            }

            ctx.BatchEventsNum = (uint)n;
            return ctx;
        }
    }
}
