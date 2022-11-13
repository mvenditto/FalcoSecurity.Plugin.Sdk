# FalcoSecurity.Plugin.Sdk

![Custom badge](https://img.shields.io/endpoint?style=for-the-badge&url=https%3A%2F%2Fgist.githubusercontent.com%2Fmvenditto%2F1f05448331025247c1c3375ebe2ba5cf%2Fraw%2F935e97d07d5e67a5cc250fb347a4c72b87ee2ec6%2Ffalco-plugin-sdk_plugin-api-version.json)
<br>

Unofficial [Falco](https://github.com/falcosecurity/falco) plugin SDK for .NET, powered by [DNNE](https://github.com/AaronRobinsonMSFT/DNNE) native exports

## Wiki

For a full example and addition information on how this works, check out the [Wiki](https://github.com/mvenditto/FalcoSecurity.Plugin.Sdk/wiki/Dummy-counter-plugin)!

## <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/2/25/NuGet_project_logo.svg/2048px-NuGet_project_logo.svg.png" width="24" />  NuGet packages
|     |      | desc  |
|-----|------|--|
| FalcoSecurity.Plugin.Sdk  | [![](https://img.shields.io/nuget/v/FalcoSecurity.Plugin.Sdk?style=flat-square&label=nuget)](https://www.nuget.org/packages/FalcoSecurity.Plugin.Sdk/)  | Core Plugin SDK types |
| FalcoSecurity.Plugin.Sdk.Generators | [![](https://img.shields.io/nuget/v/FalcoSecurity.Plugin.Sdk.Generators?style=flat-square&label=nuget)](https://www.nuget.org/packages/FalcoSecurity.Plugin.Sdk.Generators/)  | Source generators for native exports |
| FalcoSecurity.Plugin.Sdk.Template | [![](https://img.shields.io/nuget/v/FalcoSecurity.Plugin.Sdk.Template?style=flat-square&label=nuget)](https://www.nuget.org/packages/FalcoSecurity.Plugin.Sdk.Template/) | [Project template](https://github.com/mvenditto/FalcoSecurity.Plugin.Sdk/wiki/Getting-Started#The-falcoplugin-template) `dotnet new falcoplugin` |

## Dummy plugin sneak-peek
```cs
[FalcoPlugin(
    Id = 999,
    Name = "dummy_plugin",
    Description = "A dummy plugin",
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

```yaml
- rule: Dummy counter rule
  desc: Dummy counter equals 42
  condition: (dummy.counter=42)
  output: dummy.counter is 42 value=%dummy.counter
  priority: DEBUG
  source: dummy_source
  tags: [dummy]
```
<pre><samp>admin@someplace:~$ <kbd>tree /usr/share/falco</kbd>
/usr/share/falco/
└── plugins
    ├── libjson.so
    ├── libk8saudit.so
    └── dummy_plugin
        ├── plugin_native.so
        ├── FalcoSecurity.Plugin.Sdk.dll
        ├── FalcoSecurity.Plugin.Sdk.DummyPlugin.dll
        ├── Microsoft.Extensions.ObjectPool.dll
        └── FalcoSecurity.Plugin.Sdk.DummyPlugin.runtimeconfig.json</samp>
        
<samp>admin@someplace:~$ <kbd>falco --enable-source dummy_source</kbd>
Sat Nov  5 18:08:52 2022: Falco version: 0.33.0 (x86_64)
[...TRUNCATED...]
Sat Nov  5 18:08:52 2022: Enabled event sources: dummy_source
Sat Nov  5 18:08:52 2022: Opening event source 'dummy_source'
Sat Nov  5 18:08:52 2022: Opening capture with plugin 'dummy_plugin'
Sat Nov  5 18:08:52 2022: Closing event source 'dummy_source'
18:08:52.000000000: Debug dummy.counter is 42 value=42
Events detected: 1
Rule counts by severity:
   DEBUG: 1
Triggered rules by rule name:
   Dummy counter rule: 1

admin@someplace:~$ █</samp></pre>

# Note
This sdk is **Unofficial** and is not associated nor endorsed by Sysdig and falcosecurity/falco
