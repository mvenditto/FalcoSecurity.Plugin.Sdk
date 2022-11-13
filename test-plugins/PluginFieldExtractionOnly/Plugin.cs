using FalcoSecurity.Plugin.Sdk.Events;
using FalcoSecurity.Plugin.Sdk.Fields;

namespace FalcoSecurity.Plugin.Sdk.Test
{
    [FalcoPlugin(
        Id = 999,
        Name = "test_plugin",
        Description = "test_description_string!",
        Contacts = "<test test@test.com>",
        RequiredApiVersion = "2.0.0",
        Version = "1.2.3")]
    public class TestPlugin : PluginBase, IFieldExtractor		
    {
        public IEnumerable<ExtractionField> Fields => new List<ExtractionField>
        {
            new(type: "uint64", name: "test.int", desc: "an int field", display: "<int>"),
            new(type: "string", name: "test.str", desc: "a str field", display: "<str>"),
        };

        public IEnumerable<string> EventSourcesToExtract => new List<string>
        {
            "some_evt_source_1",
            "some_evt_source_2"
        };

        public void Extract(IExtractionRequest extraction, IEventReader evt)
        {
            throw new NotImplementedException();
        }
    }
}
