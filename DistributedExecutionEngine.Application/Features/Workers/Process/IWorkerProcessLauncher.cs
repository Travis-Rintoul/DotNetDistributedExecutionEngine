using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Process;

public interface IWorkerProcessLauncher
{
    public Task<Result<ProcessId, string>> LaunchAsync(WorkerId workerId, CancellationToken token);
}