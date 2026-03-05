#!/usr/bin/env bash
dotnet ef "$@" \
--project DistributedExecutionEngine.Infrastructure \
--startup-project DistributedExecutionEngine.ConsoleHost
