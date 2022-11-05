# Falco.Plugin.Sdk

Unofficial [Falco](https://github.com/falcosecurity/falco) plugin SDK for .NET, powered by [DNNE](https://github.com/AaronRobinsonMSFT/DNNE) native exports

> ✨ Early development stage alert 

For a full example check out the [Wiki](https://github.com/mvenditto/Falco.Plugin.Sdk/wiki/Dummy-counter-plugin)!

## Dummy plugin sneak-peek
```cs
[FalcoPlugin(
    Id = 999,
    Name = "dummy_plugin",
    Description = "A dummy plugin (nogen)",
    Contacts = "mvenditto",
    RequiredApiVersion = "2.0.0",
    Version = "1.0.0")]
public class Plugin: PluginBase, IEventSource, IFieldExtractor {
    public string EventSourceName => "dummy_source";

    public IEnumerable<string> EventSourcesToExtract 
        => Enumerable.Empty<string>(); // only consume ourselves event-source

    public IEnumerable<OpenParam> OpenParameters => 
        => Enumerable.Empty<string>(); // no specific open-params

    public IEnumerable<ExtractionField> Fields => new List<ExtractionField> {
        new(type: "uint64",
            name: "dummy.counter",
            display: "Counter value",
            desc: "Current value of the internal counter")
    };

    public IEventSourceInstance Open(IEnumerable<OpenParam> ? openParams) {
        return new CounterInstance();
    }
    
    public void Close(IEventSourceInstance instance) {
        instance.Dispose();
    }

    public void Extract(IExtractionRequest extraction, IEventReader evt) {
        var counter = BitConverter.ToInt32(evt.Data);
        extraction.SetValue((ulong) counter);
    }
}

public class CounterInstance: PullEventSourceInstance {
    public int Counter {get; set;}

    public CounterInstance(): base(batchSize: 10, eventSize: 8) {
        Counter = 1;
    }

    protected override void PullEvent(EventSourceInstanceContext ctx, IEventWriter evt) {
        var unixNano = (ulong) DateTimeOffset.Now.ToUnixTimeSeconds() * 1000000000;

        evt.Write(BitConverter.GetBytes(Counter));

        evt.SetTimestamp(unixNano);

        if (Counter >= 50) {
            ctx.IsEof = true;
        }

        Counter += 1;
    }
}
```

## Native plugin build pipeline

<img src="docs/build_pipeline.png" width="500"/>

# Note
This sdk is **Unofficial** and is not associated nor endorsed by Sysdig and falcosecurity/falco
