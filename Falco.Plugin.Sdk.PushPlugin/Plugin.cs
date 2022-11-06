using Falco.Plugin.Sdk.Fields;
using Falco.Plugin.Sdk.Events;

namespace Falco.Plugin.Sdk.DummyPlugin
{
    public class CounterInstance : PushEventSourceInstance
    {
        public int Counter { get; set; }

        private readonly CancellationTokenSource _cts = new();

        public CounterInstance() : base(batchSize: 10, eventSize: 8)
        {
            Counter = 1;
        }

        public override void Dispose()
        {
            base.Dispose();
            _cts.Cancel();
        }

        // simulates a producer that creates events
        public void DummyListen()
        {
            _ = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    var delay = Random.Shared.Next(0, 100);
                    await Task.Delay(delay);

                    if (Counter >= 50)
                    {
                        EventsChannel.Complete();
                    }

                    Counter++;

                    Console.WriteLine($"Counter incremented c={Counter} delay={delay}");

                    var timestamp = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds() * 1000000000;
                    var data = BitConverter.GetBytes(Counter);

                    await EventsChannel.WriteAsync(new(timestamp, data), _cts.Token);
                }
            }, _cts.Token);
        }
    }

    [FalcoPlugin(
    Id = 999,
        Name = "dummy_plugin",
        Description = "An example 'push mode' plugin",
        Contacts = "mvenditto",
        RequiredApiVersion = "2.0.0",
        Version = "1.0.0")]
    public class Plugin : PluginBase, IEventSource, IFieldExtractor
    {
        public string EventSourceName => "dummy_source_2";

        public IEnumerable<string> EventSourcesToExtract 
            => Enumerable.Empty<string>();

        public IEnumerable<OpenParam> OpenParameters
            => Enumerable.Empty<OpenParam>();

        public IEnumerable<ExtractionField> Fields => new List<ExtractionField>
        {
            new(type: "uint64",
                name: "push.counter",
                display: "Counter value",
                desc:  "Current value of the internal counter")
        };

        public void Close(IEventSourceInstance instance)
        {
            instance.Dispose();
        }

        public IEventSourceInstance Open(IEnumerable<OpenParam>? openParams)
        {
            var i = new CounterInstance();
            i.DummyListen();
            return i;
        }

        public void Extract(IExtractionRequest extraction, IEventReader evt)
        {
            var counter = BitConverter.ToInt32(evt.Data);
            extraction.SetValue((ulong)counter);
        }
    }
}
