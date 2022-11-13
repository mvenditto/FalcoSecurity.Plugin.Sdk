using FalcoSecurity.Plugin.Sdk.Events;

namespace FalcoSecurity.Plugin.Sdk.Test
{
    internal class TestPullEventSource : PullEventSourceInstance
    {
        public int Counter { get; private set; }

        public TestPullEventSource(int batchSize, int eventSize): base(batchSize, eventSize)
        {
        }

        protected override void PullEvent(EventSourceInstanceContext ctx, IEventWriter evt)
        {
            Counter += 1; 
            
            if (Counter >= EventSourceConsts.DefaultBatchSize)
            {
                ctx.IsEof = true;
            }

            evt.Write(BitConverter.GetBytes(Counter));
        }
    }
}
