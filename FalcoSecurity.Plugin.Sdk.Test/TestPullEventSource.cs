using FalcoSecurity.Plugin.Sdk.Events;

namespace FalcoSecurity.Plugin.Sdk.Test
{
    internal class TestPullEventSource : PullEventSourceInstance
    {
        public int Counter { get; set; }

        public TestPullEventSource(int batchSize, int eventSize): base(batchSize, eventSize)
        {
        }

        public Action<EventSourceInstanceContext, IEventWriter>? PullEventDelegate { get; set; }

        protected override void PullEvent(EventSourceInstanceContext ctx, IEventWriter evt)
        {
            PullEventDelegate?.Invoke(ctx, evt);
        }
    }
}
