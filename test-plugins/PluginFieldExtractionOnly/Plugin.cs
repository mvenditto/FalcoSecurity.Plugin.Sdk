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
            new(type: "uint64", name: "test.u64", desc: "an integer field", display: "<u64>"),
            new(type: "string", name: "test.str", desc: "a string field", display: "<str>"),
            new(type: "uint64", name: "test.[u64]", isList: true, desc: "an integer[] field", display: "<u64[]>"),
            new(type: "string", name: "test.[str]", isList: true, desc: "a string[] field", display: "<str[]>")
        };

        public IEnumerable<string> EventSourcesToExtract => new List<string>
        {
            "some_evt_source_1",
            "some_evt_source_2"
        };

        public void Extract(IExtractionRequest extraction, IEventReader evt)
        {
            var counter = (ulong) BitConverter.ToInt32(evt.Data);

            if (extraction.FieldName == "test.u64")
            {
                extraction.SetValue(counter);
            }
            else if (extraction.FieldName == "test.str")
            {
                extraction.SetValue($"Counter = {counter}");
            }
            else if (extraction.FieldName == "test.[u64]")
            {
                var elementsCount = (int) Math.Max(1, counter);
                var elements = new ulong[elementsCount];
                for (var i = 0; i < elementsCount; i++)
                {
                    elements[i] = (ulong) i;
                }
                extraction.SetValue(elements);
            }
            else if (extraction.FieldName == "test.[str]")
            {
                var elementsCount = (int)Math.Max(1, counter);
                var elements = new string[elementsCount];
                for (var i = 0; i < elementsCount; i++)
                {
                    elements[i] = $"Counter: {i}";
                }
                extraction.SetValue(elements);
            }
        }
    }
}
