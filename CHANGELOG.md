# [0.4.1](https://github.com/mvenditto/FalcoSecurity.Plugin.Sdk/tree/8d839d43f725be442e5126159a0d6515259e930c/FalcoSecurity.Plugin.Sdk) (2022-11-19)
### Release Highlights
  - switched from using memory methods from [`System.Runtime.InteropServices.Marshal`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.marshal?view=net-6.0) to the newest (NET6+) [`System.Runtime.InteropServices.NativeMemory`](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.nativememory?view=net-6.0), which is a portable wrapper for C-runtime `malloc`, `free`, etc. ([#266c228](https://github.com/mvenditto/FalcoSecurity.Plugin.Sdk/commit/266c228e7d30737110ba8ce65747c9f0c4c1aa76))
  - fixed native memory leak in `ExtractionRequest`
### Breaking Changes
|                              |                            |
|------------------------------|----------------------------|
|`IEventBatch.UnderlyingArray` | `IntPtr` -> `PluginEvent*` |
|`PluginEvent.Data`            | `IntPtr` -> `void*`        |
|`IExtractionRequest.SetPtr`   | (`IntPtr` ptr) -> (`PluginExtractField*` ptr)|
|`IPlugin`                     | +`string? ConfigRaw` |
