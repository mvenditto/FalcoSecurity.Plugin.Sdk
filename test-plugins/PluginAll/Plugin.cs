using FalcoSecurity.Plugin.Sdk.Events;
using FalcoSecurity.Plugin.Sdk.Fields;

namespace FalcoSecurity.Plugin.Sdk.Test
{
    internal class EventSourceIntance : PullEventSourceInstance
    {
        private int _counter = 0;

        public EventSourceIntance() : base(10, sizeof(int))
        {
            TimeoutMs = 0;
        }

        protected override void PullEvent(EventSourceInstanceContext ctx, IEventWriter evt)
        {
            evt.SetTimestamp(ulong.MaxValue);
            evt.Write(BitConverter.GetBytes(_counter));
            _counter += 1;
        }
    }

    [FalcoPlugin(
        Id = 999,
        Name = "test_plugin",
        Description = "test_description_string!",
        Contacts = "<test test@test.com>",
        RequiredApiVersion = "2.0.0",
        Version = "1.2.3")]
    public class TestPlugin : PluginBase, IEventSource, IFieldExtractor		
    {
        public string EventSourceName => "test_eventsource";

        public IEnumerable<OpenParam> OpenParameters
            => Enumerable.Empty<OpenParam>();

        public IEnumerable<ExtractionField> Fields => new List<ExtractionField> { 
            new("uint64", "dummy", "dummy field")
        };

        public IEnumerable<string> EventSourcesToExtract
            => Enumerable.Empty<string>();

        public IEventSourceInstance Open(IEnumerable<OpenParam>? openParams)
        {
            return new EventSourceIntance();
        }

        public void Close(IEventSourceInstance instance)
        {
            instance.Dispose();
        }

        public void Extract(IExtractionRequest extraction, IEventReader evt)
        {
            extraction.SetValue((ulong) BitConverter.ToInt32(evt.Data));
        }
    }
}
