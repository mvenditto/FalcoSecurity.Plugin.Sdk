# FalcoSecurity.Plugin.Sdk
[![Build Status](https://img.shields.io/appveyor/build/mvenditto/falcosecurity-plugin-sdk.svg?style=for-the-badge&logo=appveyor&logoColor=white)](https://ci.appveyor.com/project/mvenditto/falcosecurity-plugin-sdk)
[![AppVeyor Tests](https://img.shields.io/appveyor/tests/mvenditto/falcosecurity-plugin-sdk?&compact_message&logo=appveyor&logoColor=white&style=for-the-badge)](https://ci.appveyor.com/project/mvenditto/falcosecurity-plugin-sdk/build/tests)
[![Coverage](https://img.shields.io/codecov/c/github/mvenditto/FalcoSecurity.Plugin.Sdk?style=for-the-badge&token=KNEH3735KU&logo=codecov&logoColor=white)](https://app.codecov.io/gh/mvenditto/FalcoSecurity.Plugin.Sdk) 
![Codacy grade](https://img.shields.io/codacy/grade/71314d20dbc64028bc80d4291272af2d?style=for-the-badge&logo=codacy) 
![Custom badge](https://img.shields.io/endpoint?style=for-the-badge&url=https%3A%2F%2Fgist.githubusercontent.com%2Fmvenditto%2F1f05448331025247c1c3375ebe2ba5cf%2Fraw%2F935e97d07d5e67a5cc250fb347a4c72b87ee2ec6%2Ffalco-plugin-sdk_plugin-api-version.json&logo=data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHJvbGU9ImltZyIgdmlld0JveD0iNDguOTEgLTEuMDkgNDcyLjE4IDU3Ni42OCI+PGRlZnM+PHN0eWxlPi5jbHMtMXtmaWxsOiNmZmZ9PC9zdHlsZT48L2RlZnM+PHBhdGggZD0iTTQ0OS42MDcgMjQ0LjY5OGw2MC44OS0xMC42MTNjLTkuOTM3LTEwLjA4Ny01MS4yNzgtNDAuNjI3LTYwLjctMzEuMjRxLTU5LjE1NCA1OC45MjYtMTE4LjM3NiAxMTcuNzg2TDE0Ni4wNzUgNTA1Ljk3N2wtLjQ1NS0uNDU0Yy0xMC42MTQgMTAuNjUtMjEuMTU4IDIxLjM3Mi0zMS4zOTMgMzIuMzc3LTUuMzkzIDUuOC03LjM4MyAxMy4xNzMtMS4xNjUgMTkuODcgNi41MDUgNy4wMTIgMTQuMzQ1IDYuNTg3IDIxLjUyOCAxLjI2NiA1LjM3Ny0zLjk4MSAxMC4wNjMtOC45NDggMTQuODIzLTEzLjcwNXE2LjI2LTYuMjYgMTIuNTI0LTEyLjUxNiAxNy4zNzMtMTcuMzYzIDM0Ljc0OS0zNC43MjQgMTI2LjYyNy0xMjYuNTI5IDI1Mi45Mi0yNTMuMzkzeiIgY2xhc3M9ImNscy0xIi8+PHBhdGggZD0iTTQ3OC43ODQgMjkyLjQzN2ExMi45MjMgMTIuOTIzIDAgMCAwLTkuMDgyLTEyLjU1Yy05LjctMy41MDYtMTYuMzU3IDIuMTY4LTIyLjU5OCA4LjQwNnEtMTEzLjg1IDExMy44MDMtMjI3LjY3IDIyNy42MzYtOC4zODUgOC4zODktMTYuNzc1IDE2Ljc3M2MtNC4wMjMgNC4wMjUtOC4xMTcgOC40ODItOS40MTYgMTMuMjkyLTEuMDMgMy44MS0uMzA3IDcuODQgMy42IDEyLjA1NCA5Ljc1OCAxMC41MjQgMTguOSAyLjk4OCAyNi42MzktNC43NDNxMTIzLjU3Mi0xMjMuNDUyIDI0Ny4wNjYtMjQ2Ljk4M2MzLjkxNS0zLjkxNSA3LjgyNS03Ljg2MyA4LjIzNi0xMy44ODV6bS0xNjIuOTI0LTkuOTExbDE3LjE0My0xNy4xNDFjLTEuMzkxLTQuMzI2LTQuOTA4LTguNDE0LTguNjEtMTIuMTJRMjA2LjgyMiAxMzUuNjMyIDg5LjE2OCAxOC4wODZjLTQuMzg3LTQuMzg0LTkuMDc1LTguMzQxLTE1Ljg3LTguMDczLTUuMzI5LjI3NS05LjU5NyAyLjU1Mi0xMS42MiA3LjU1My0zLjU3NiA4LjgzMy45OTUgMTUuMjk3IDYuODkgMjEuMTk1cTExNy41ODUgMTE3LjYxNiAyMzUuMTg0IDIzNS4yMTVjMy43MDcgMy43MDYgNy43OTYgNy4xOCAxMi4xMDggOC41NXptNzkuODYzIDE4Mi42NDdjMjUuMjA0LTI0LjI1IDQ5LjY3Ny00OS4yNiA3NC4yNy03NC4xNCA3LjI1Ni03LjM0NCAxMy43LTE2LjEyNCAzLjkxNy0yNS4zNi04Ljc4Mi04LjI5NS0xNy4xNjktMi41MS0yNC4wNzIgNC4zNTMtMzEuMzM0IDMxLjE1Ny02Mi40OCA2Mi41MDQtOTMuODE4IDkzLjY1NS0xMC45NjEgMTAuODkzLTEwLjQ1NSAyMS4wNTcuNCAzMS43MjEgMTguMzg1IDE4LjA2NyAzNi43NDQgMzYuMTczIDU0LjU4NSA1NC43NyAxMC4wMDQgMTAuNDMgMjAuOTMgMTYuOTUxIDM1LjA0MiAxNC4xOTUgMy4yNTkuMjEgNS43MzguNjkyIDguMTUuNDY3IDEwLjMwMy0uOTU4IDIzLjMwNy0uMjgyIDIzLjYzNC0xNC40NzkuMzM2LTE0LjY0NS0xMi4zMzEtMTQuNzA5LTIyLjk0My0xNC4zMTMtMTAuNzc3LjQwMy0xOC42ODMtNC4wMzEtMjUuODQ1LTExLjU2NC0xMC45OTEtMTEuNTYyLTIxLjk5LTIzLjE5LTMzLjg3LTMzLjgwOS0xMC41MTMtOS4zOTgtOS4wMTgtMTYuMjkyLjU1LTI1LjQ5NnpNMjYwLjY1NSAzMTUuMDY1YzMuODE4IDMuODAyIDguMzk0IDcuNjg3IDEzLjM0NyA5LjMyMWwxNy4wNjMtMTcuMDYzYy0yLjE2My02LjE4OC03LjMxNi0xMC4zMi0xMS43ODQtMTQuNzk4cS03Mi4wNjItNzIuMjI2LTE0NC4yNTYtMTQ0LjMyYy05Ljk2NS05Ljk2LTE5Ljg3Mi0xOS45OTMtMzAuMTQ5LTI5LjYzLTYuMDQtNS42NjctMTMuMjEzLTcuMTI0LTE5LjY1Mi0uNzQyLTYuMTMgNi4wNzMtNS42OSAxMy4yMi0uODU5IDIwLjAyNGE2Ni4xNDkgNjYuMTQ5IDAgMCAwIDcuMjIzIDguMzAzcTg0LjQ2IDg0LjUyNiAxNjkuMDY3IDE2OC45MDV6bS02NC4wMDkgMjAuNTE4YzkuMSA5LjA3NiAxOC4xNTMgMTguMjE0IDI3LjU4NSAyNi45MzZhMTcuNjc0IDE3LjY3NCAwIDAgMCA3LjQwOCA0LjIzMWwxNy44NS0xNy44NWMtMS4xNzItNC42LTQuMjI2LTguMjQxLTcuNjMtMTEuNjVxLTU0LjQ5NS01NC41OTQtMTA5LjExLTEwOS4wNjVhMzMuMzI4IDMzLjMyOCAwIDAgMC0xMC40MjUtNy4yNjRjLTUuOTcxLTIuNDQ3LTEyLjEyNi0yLjA1My0xNi44NzIgMy4xNzQtNC43OTMgNS4yNzktNC43MyAxMS4zNTMtMS4wMTEgMTYuOTA4YTk2LjA4IDk2LjA4IDAgMCAwIDExLjcyMiAxNC4wMjhxNDAuMTEzIDQwLjQwNCA4MC40ODMgODAuNTUyeiIgY2xhc3M9ImNscy0xIi8+PC9zdmc+) 
![DNNE](https://img.shields.io/badge/DNNE-1.0.32-purple?&style=for-the-badge)

Unofficial [Falco](https://github.com/falcosecurity/falco) plugin SDK for .NET, powered by [DNNE](https://github.com/AaronRobinsonMSFT/DNNE) native exports

## Wiki

For a full example and addition information on how this works, check out the [Wiki](https://github.com/mvenditto/FalcoSecurity.Plugin.Sdk/wiki/Dummy-counter-plugin)!

## <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/2/25/NuGet_project_logo.svg/2048px-NuGet_project_logo.svg.png" width="24" />  NuGet packages
|     |      | desc  | changelog |
|-----|------|-------|-----------|
| FalcoSecurity.Plugin.Sdk  | [![](https://img.shields.io/nuget/v/FalcoSecurity.Plugin.Sdk?style=flat-square&label=nuget)](https://www.nuget.org/packages/FalcoSecurity.Plugin.Sdk/)  | Core Plugin SDK types |  |     
| FalcoSecurity.Plugin.Sdk.Generators | [![](https://img.shields.io/nuget/v/FalcoSecurity.Plugin.Sdk.Generators?style=flat-square&label=nuget)](https://www.nuget.org/packages/FalcoSecurity.Plugin.Sdk.Generators/)  | Source generators for native exports | [CHANGELOG.md](https://github.com/mvenditto/FalcoSecurity.Plugin.Sdk/blob/master/FalcoSecurity.Plugin.Sdk.Generators/CHANGELOG.md) |
| FalcoSecurity.Plugin.Sdk.Template | [![](https://img.shields.io/nuget/v/FalcoSecurity.Plugin.Sdk.Template?style=flat-square&label=nuget)](https://www.nuget.org/packages/FalcoSecurity.Plugin.Sdk.Template/) | [Project template](https://github.com/mvenditto/FalcoSecurity.Plugin.Sdk/wiki/Getting-Started#The-falcoplugin-template) `dotnet new falcoplugin` |  |

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
