using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Supervision;

public interface IWorkerProcessLauncher
{
    public Task<Result<ProcessId, string>> LaunchAsync(WorkerId workerId, CancellationToken token);
}