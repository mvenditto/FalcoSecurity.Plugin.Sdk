#! /usr/bin/env pwsh

dotnet test `
  --no-build `
  --diag:logs/logs.txt `
  --blame `
  --blame-crash `
  --logger "console;verbosity=detailed"
