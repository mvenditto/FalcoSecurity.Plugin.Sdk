dotnet build ../../test-plugins/PluginAll/FalcoSecurity.Plugin.Sdk.Test.PluginAllCaps.csproj --configuration Release --runtime linux-x64 --no-self-contained &&
dotnet build ../../test-plugins/PluginEventSourceOnly/FalcoSecurity.Plugin.Sdk.Test.PluginEventSourceOnly.csproj --configuration Release --runtime linux-x64 --no-self-contained &&
dotnet build ../../test-plugins/PluginFieldExtractionOnly/FalcoSecurity.Plugin.Sdk.Test.PluginFieldExtractionOnly.csproj --configuration Release --runtime linux-x64 --no-self-contained &&
cmake -S . -B build && cmake --build build &&
chmod +x ./build/integration_test &&
./build/integration_test