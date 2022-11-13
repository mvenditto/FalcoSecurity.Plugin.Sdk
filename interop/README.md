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
[==========] Running 14 tests from 3 test suites.
[----------] Global test environment set-up.
[----------] 8 tests from PluginBaseTest
[ RUN      ] PluginBaseTest.PluginLoad
[       OK ] PluginBaseTest.PluginLoad (5 ms)
[ RUN      ] PluginBaseTest.PluginHasRequiredSymbols
[       OK ] PluginBaseTest.PluginHasRequiredSymbols (5 ms)
[ RUN      ] PluginBaseTest.PluginGetName
[       OK ] PluginBaseTest.PluginGetName (101 ms)
[ RUN      ] PluginBaseTest.PluginGetVersion
[       OK ] PluginBaseTest.PluginGetVersion (11 ms)
[ RUN      ] PluginBaseTest.PluginGetContacts
[       OK ] PluginBaseTest.PluginGetContacts (10 ms)
[ RUN      ] PluginBaseTest.PluginGetDescription
[       OK ] PluginBaseTest.PluginGetDescription (11 ms)
[ RUN      ] PluginBaseTest.PluginHasExpectedCaps
[       OK ] PluginBaseTest.PluginHasExpectedCaps (3 ms)
[ RUN      ] PluginBaseTest.PluginGetEventSource
[       OK ] PluginBaseTest.PluginGetEventSource (11 ms)
[----------] 8 tests from PluginBaseTest (160 ms total)

[----------] 3 tests from PluginEventSourceOnlyTest
[ RUN      ] PluginEventSourceOnlyTest.PluginGetId
[       OK ] PluginEventSourceOnlyTest.PluginGetId (23 ms)
[ RUN      ] PluginEventSourceOnlyTest.PluginGetEventSource2
[       OK ] PluginEventSourceOnlyTest.PluginGetEventSource2 (11 ms)
[ RUN      ] PluginEventSourceOnlyTest.PluginHasEventSourcingCapOnly
[       OK ] PluginEventSourceOnlyTest.PluginHasEventSourcingCapOnly (3 ms)
[----------] 3 tests from PluginEventSourceOnlyTest (38 ms total)

[----------] 3 tests from PluginFieldExtractionOnlyTest
[ RUN      ] PluginFieldExtractionOnlyTest.PluginHasEventSourcingCapOnly
[       OK ] PluginFieldExtractionOnlyTest.PluginHasEventSourcingCapOnly (4 ms)
[ RUN      ] PluginFieldExtractionOnlyTest.PluginGetFields
[       OK ] PluginFieldExtractionOnlyTest.PluginGetFields (45 ms)
[ RUN      ] PluginFieldExtractionOnlyTest.PluginGetExtractionSources
[       OK ] PluginFieldExtractionOnlyTest.PluginGetExtractionSources (12 ms)
[----------] 3 tests from PluginFieldExtractionOnlyTest (62 ms total)

[----------] Global test environment tear-down
[==========] 14 tests from 3 test suites ran. (261 ms total)
[  PASSED  ] 14 tests.
```
