using FalcoSecurity.Plugin.Sdk.Events;
using System.Threading.Channels;

namespace FalcoSecurity.Plugin.Sdk.Test
{
    internal class TestPushEventSource : PushEventSourceInstance
    {
        public int Counter { get; set; }

        public Action<ChannelWriter<PushEvent>, Action> EventProducer { get; set; }

        public TestPushEventSource(int batchSize, int eventSize) : base(batchSize, eventSize)
        {
            TimeoutMs = 0;
        }

        public void Start()
        {
            EventProducer?.Invoke(EventsChannel, () => Eof(null));
        }
    }
}
