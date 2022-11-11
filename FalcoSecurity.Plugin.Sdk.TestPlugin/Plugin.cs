using FalcoSecurity.Plugin.Sdk.Events;
using FalcoSecurity.Plugin.Sdk.Fields;

namespace FalcoSecurity.Plugin.Sdk.Test
{
    [FalcoPlugin(
        Id = 999,
        Name = "test_plugin",
        Description = "test_description_string",
        Contacts = "me",
        RequiredApiVersion = "2.0.0",
        Version = "1.2.3")]
    public class TestPlugin : PluginBase, IEventSource, IFieldExtractor		
    {
        public string EventSourceName => "test_eventsource";

        public IEnumerable<OpenParam> OpenParameters
            => Enumerable.Empty<OpenParam>();

        public IEnumerable<ExtractionField> Fields
            => Enumerable.Empty<ExtractionField>();


        public IEnumerable<string> EventSourcesToExtract
            => Enumerable.Empty<string>();

        public IEventSourceInstance Open(IEnumerable<OpenParam>? openParams)
        {
            throw new NotImplementedException();
        }

        public void Close(IEventSourceInstance instance)
        {
            instance.Dispose();
        }

        public void Extract(IExtractionRequest extraction, IEventReader evt)
        {
            throw new NotImplementedException();
        }
    }
}
