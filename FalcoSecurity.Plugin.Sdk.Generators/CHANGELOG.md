# [0.3.6](https://github.com/mvenditto/FalcoSecurity.Plugin.Sdk/tree/4dc0a1a1dcd1f87ff5b644e1af84b1a353d9440f/FalcoSecurity.Plugin.Sdk.Generators) (2022-11-13)
### Release Highlights
  - rewrite of the Roslyn source generator to use [Scriban](https://github.com/scriban/scriban) template engine:
      - more robust, maintainable and less error-prone
      - the template (mixed C# and Scriban) resides now in `PluginNativeExports.sbncs`
  - better generated code overhaul:
      - output only the least amount of code needed
      - better conditional blocks handling
      - only export required symbols
  - added many interop tests ([#f9ec488](https://github.com/mvenditto/FalcoSecurity.Plugin.Sdk/commit/f9ec4888c6f35cd4b6d26b92a20800d0bff23baa))
    - added three test plugins (`test-plugins` folder) to test the main capability combinations
### Bugfixes
  - fixed missing `plugin_get_extract_event_sources` export ([#8e17b98](https://github.com/mvenditto/FalcoSecurity.Plugin.Sdk/commit/8e17b9867531ced78367e7006bc86b88daba5f19))
  - fixed a minor memory leak in `Destroy()`
