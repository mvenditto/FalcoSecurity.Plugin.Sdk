using System.Diagnostics;

namespace FalcoSecurity.Plugin.Sdk.Events
{
    public abstract class PullEventSourceInstance: BaseEventSourceInstance
    {
        private bool _eofReached = false;

        private readonly Stopwatch _stopwatch;

        private readonly EventSourceInstanceContext Context;

        protected PullEventSourceInstance(int batchSize, int eventSize):
            base(batchSize, eventSize)
        {
            _stopwatch = new Stopwatch();
            Context = new();
        }

        public override void Dispose()
        {
            base.Dispose();
            _stopwatch.Stop();
            GC.SuppressFinalize(this);
        }

        abstract protected void PullEvent(EventSourceInstanceContext ctx, IEventWriter evt);

        override public EventSourceInstanceContext NextBatch()
        {
            if (_eofReached)
            {
                Context.IsEof = true;
                Context.BatchEventsNum = 0;
                return Context;
            }

            if (TimeoutMs > 0)
            {
                _stopwatch.Restart();
            }

            var n = 0;

            for (;n < EventBatch.Length; n++)
            {
                if (TimeoutMs > 0 && _stopwatch.ElapsedMilliseconds > TimeoutMs)
                {
                    // flush a partial batch
                    _stopwatch.Stop();
                    Context.HasTimeout = true;
                    Context.BatchEventsNum = (uint) n;
                    return Context;
                }

                if (Context.IsEof)
                {
                    _eofReached = true;
                    return Context;
                }

                try
                {
                    PullEvent(Context, EventBatch.Get(n));
                }
                catch (Exception ex)
                {
                    _eofReached = true;
                    Context.HasFailure = true;
                    Context.Error = ex.ToString();
                    Context.BatchEventsNum = 0;
                    return Context;
                }
            }

            Context.BatchEventsNum = (uint) n;
            return Context;
        }
    }
}
