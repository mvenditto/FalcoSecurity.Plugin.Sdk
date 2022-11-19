#!/bin/sh

dotnet test \
    --no-build \
    --diag:logs/logs.txt \
    --blame \
    --blame-crash \
    --logger "console;verbosity=detailed" \
    --runtime linux-x64
