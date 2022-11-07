#if eventSourcingCapability
using Falco.Plugin.Sdk.Events;
#endif
#if fieldExtractionCapability
using Falco.Plugin.Sdk.Fields;
#endif

namespace Falco.Plugin.TPluginName
{
    [FalcoPlugin(
        Id = PLUGIN_ID,
        Name = "PLUGIN_NAME",
        Description = "PLUGIN_DESC",
        Contacts = "me",
        RequiredApiVersion = "PLUGIN_REQ_API_VERSION",
        Version = "PLUGIN_VERSION")]
#if (eventSourcingCapability && fieldExtractionCapability)
    public class TPluginName : PluginBase, IEventSource, IFieldExtractor		
#elseif eventSourcingCapability
    public class TPluginName : PluginBase, IEventSource
#else
    public class TPluginName : PluginBase, IFieldExtractor
#endif
    {
#if eventSourcingCapability
        public string EventSourceName => "PLUGIN_EVTSOURCE";

        public IEnumerable<OpenParam> OpenParameters 
            => Enumerable.Empty<OpenParam>();
#endif

#if fieldExtractionCapability	
        public IEnumerable<ExtractionField> Fields 
            => Enumerable.Empty<ExtractionField>();


        public IEnumerable<string> EventSourcesToExtract 
            => Enumerable.Empty<string>();
#endif
            
#if eventSourcingCapability
        public IEventSourceInstance Open(IEnumerable<OpenParam>? openParams)
        {
            throw new NotImplementedException();
        }
        
        public void Close(IEventSourceInstance instance)
        {
            instance.Dispose();
        }
#endif

#if fieldExtractionCapability	
        public void Extract(IExtractionRequest extraction, IEventReader evt)
        {
            throw new NotImplementedException();
        }
#endif
    }
}
