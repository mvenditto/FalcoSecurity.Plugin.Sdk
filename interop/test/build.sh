dotnet build ../../Falco.Plugin.Sdk.TestPlugin/Falco.Plugin.Sdk.TestPlugin.csproj --configuration Release --runtime linux-x64 --no-self-contained &&
cmake -S . -B build && cmake --build build &&
chomod +x ./build/integration_test &&
./build/integration_test