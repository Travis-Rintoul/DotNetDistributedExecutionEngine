using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Process;

public interface IWorkerProcessTerminator
{
    public Task TryTerminateAsync(ProcessId processId, CancellationToken cancellationToken);
}