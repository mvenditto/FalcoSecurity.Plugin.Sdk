using Falco.Plugin.Sdk.Fields;
using Falco.Plugin.Sdk.Events;

namespace Falco.Plugin.Sdk.DummyPlugin
{
    [FalcoPlugin(
        Id = 999,
        Name = "my_plugin",
        Description = "A new Falco plugin",
        Contacts = "author",
        RequiredApiVersion = "2.0.0",
        Version = "1.0.0")]
    public class Plugin : PluginBase, IEventSource, IFieldExtractor
    {
        public string EventSourceName => "my_source";

        public IEnumerable<string> EventSourcesToExtract 
            => Enumerable.Empty<string>();

        public IEnumerable<OpenParam> OpenParameters 
			=> Enumerable.Empty<OpenParam>();

        public IEnumerable<ExtractionField> Fields 
			=> Enumerable.Empty<ExtractionField>();

        public void Close(IEventSourceInstance instance)
        {
            instance.Dispose();
        }

        public IEventSourceInstance Open(IEnumerable<OpenParam>? openParams)
        {
            throw new NotImplementedException();
        }

        public void Extract(IExtractionRequest extraction, IEventReader evt)
        {
            
        }
    }
}
