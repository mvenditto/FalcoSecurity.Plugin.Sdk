# Interop Test and benchmarks

The `test` folder contains a test project aimed to verify that a plugin native library produced by building a plugin:
  - can be loaded from C/CPP
  - exports all the required symbols
  - all the exported function can be called and correctly execute
and in general, any aspect related to the interop with the "native" side (aka the Falco plugin framework).

The project is based on `cmake` + [googletest](https://github.com/google/googletest).

The `build.sh` script build the `Falco.Plugin.Sdk.TestPlugin` project and executes the test suite on the produced library.

### Example output
```
[==========] Running 4 tests from 1 test suite.
[----------] Global test environment set-up.
[----------] 4 tests from PluginIntegrationTest
[ RUN      ] PluginIntegrationTest.PluginLoad
[       OK ] PluginIntegrationTest.PluginLoad (4 ms)
[ RUN      ] PluginIntegrationTest.PluginHasRequiredSymbols
[       OK ] PluginIntegrationTest.PluginHasRequiredSymbols (4 ms)
[ RUN      ] PluginIntegrationTest.PluginGetName
[       OK ] PluginIntegrationTest.PluginGetName (124 ms)
[ RUN      ] PluginIntegrationTest.PluginGetVersion
[       OK ] PluginIntegrationTest.PluginGetVersion (13 ms)
[----------] 4 tests from PluginIntegrationTest (147 ms total)

[----------] Global test environment tear-down
[==========] 4 tests from 1 test suite ran. (147 ms total)
[  PASSED  ] 4 tests.
```
