using System.ComponentModel;
using System.Text.Json.Serialization;
using Falco.Plugin.Sdk.Events;

namespace Falco.Plugin.Sdk
{
    public class Config
    {
        [JsonPropertyName("flushInterval")]
        [Description("Flush Interval in ms (Default: 30)")]
        public long FlushInterval { get; set; } = 30;
    }

    [FalcoPlugin(
        Id = 42,
        Name = "DummyPlugin",
        Description = "A dummy plugin",
        Contacts = "mvenditto",
        RequiredApiVersion = "2.0.0",
        Version = "1.0.0")]
    public class Plugin: PluginBase<Config>, IEventSource, IFieldExtractor
    {
        public string EventSourceName => "dummy_source";

        public IList<string> EventSourcesToConsume => new List<string> 
        { 
            "some_source_1",
            "some_source_2"
        };

        public IList<OpenParam> OpenParameters => new List<OpenParam>
        {
            new(value: "resource1", desc: "An example of openable resource"),
            new(value: "resource2", desc: "Another example of openable resource"),
            new(value: "res1;res2;res3", desc: "Some names", separator: ";"),
        };

        public IList<ExtractionField> ExtractFields => new List<ExtractionField>
        {
            new(type: "uint64", name: "example.count", display: "Counter value", desc:  "Current value of the internal counter"),
            new(type: "string", name: "example.countstr", display: "Counter string value", desc:  "CurrentString represetation of current value of the internal counter")
        };

        public void Close(IEventSourceInstance instance)
        {
            instance.Dispose();
        }

        public IEventSourceInstance Open(IList<OpenParam> openParams)
        {
            var instance = new BaseEventSourceInstance
            {
                EventPool = new EventPool(10, 64),
                State = 0,
            }; 
            
            return instance;
        }

        /*
        public int GetNextBatch(EventSourceInstance instance)
        {
            var eventPool = instance.EventPool;

            for (var i = 0; i < eventPool.Length; i++)
            {
                var evt = eventPool.Get(i);
                instance.State = (int)(instance.State ?? 0) + 1;
                var unixNano = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds() * 1000000000;
                evt.Write(BitConverter.GetBytes(unixNano));
                evt.SetTimestamp(unixNano);
            }

            return eventPool.Length;
        }
        */
    }
}
