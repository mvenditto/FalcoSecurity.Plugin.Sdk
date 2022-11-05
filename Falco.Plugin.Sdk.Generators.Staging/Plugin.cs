using Falco.Plugin.Sdk.Events;
using Falco.Plugin.Sdk.Fields;

namespace Falco.Plugin.Sdk
{
    public class CounterInstance : PullEventSourceInstance
    {
        public int Counter { get; set; }

        public CounterInstance(): base(batchSize: 10, eventSize: 8)
        {
            Counter = 1;
        }

        protected override void PullEvent(EventSourceInstanceContext ctx, IEventWriter evt)
        {
            var unixNano = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds() * 1000000000;

            evt.Write(BitConverter.GetBytes(Counter));

            evt.SetTimestamp(unixNano);

            if (Counter >= 50)
            {
                ctx.IsEof = true;
            }

            Counter += 1;
        }
    }

    [FalcoPlugin(
        Id = 999,
        Name = "dummy_plugin",
        Description = "A dummy plugin (nogen)",
        Contacts = "mvenditto",
        RequiredApiVersion = "2.0.0",
        Version = "1.0.0")]
    public class Plugin: PluginBase, IEventSource, IFieldExtractor
    {
        private const string _evtSource = "dummy_source";

        public string EventSourceName => _evtSource;

        public IList<string> EventSourcesToExtract => new List<string> 
        {
           _evtSource
        };

        public IList<OpenParam> OpenParameters => new List<OpenParam>
        {
            new(value: "file:///hello-world.bin", 
                desc: "A resource that can be opened by this plugin. This is not used here and just serves an example.")
        };

        public IList<ExtractionField> Fields => new List<ExtractionField>
        {
            new(type: "uint64", name: "dummy.counter", display: "Counter value", desc:  "Current value of the internal counter")
        };

        public void Close(IEventSourceInstance instance)
        {
            instance.Dispose();
        }

        public IEventSourceInstance Open(IList<OpenParam>? openParams)
        {
            return new CounterInstance
            {
                TimeoutMs = 3
            };
        }

        public void Extract(IExtractionRequest extraction, IEventReader evt)
        {
            var counter = BitConverter.ToInt32(evt.Data);
            extraction.SetValue((ulong) counter);
        }
    }
}
