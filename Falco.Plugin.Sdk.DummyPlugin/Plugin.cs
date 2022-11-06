using Falco.Plugin.Sdk.Fields;
using Falco.Plugin.Sdk.Events;

namespace Falco.Plugin.Sdk.DummyPlugin
{
    public class CounterInstance : PullEventSourceInstance
    {
        public int Counter { get; set; }

        public CounterInstance() : base(batchSize: 10, eventSize: 8)
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
        Description = "A dummy plugin",
        Contacts = "mvenditto",
        RequiredApiVersion = "2.0.0",
        Version = "1.0.0")]
    public class Plugin : PluginBase, IEventSource, IFieldExtractor
    {
        public string EventSourceName => "dummy_source";

        public IEnumerable<string> EventSourcesToExtract 
            => Enumerable.Empty<string>();

        public IEnumerable<OpenParam> OpenParameters => new List<OpenParam>
        {
            new(value: "file:///hello-world.bin",
                desc: "This is not used here and just serves an example.")
        };

        public IEnumerable<ExtractionField> Fields => new List<ExtractionField>
        {
            new(type: "uint64", 
                name: "dummy.counter", 
                display: "Counter value", 
                desc:  "Current value of the internal counter")
        };

        public void Close(IEventSourceInstance instance)
        {
            instance.Dispose();
        }

        public IEventSourceInstance Open(IEnumerable<OpenParam>? openParams)
        {
            return new CounterInstance();
        }

        public void Extract(IExtractionRequest extraction, IEventReader evt)
        {
            var counter = BitConverter.ToInt32(evt.Data);
            extraction.SetValue((ulong)counter);
        }
    }
}
