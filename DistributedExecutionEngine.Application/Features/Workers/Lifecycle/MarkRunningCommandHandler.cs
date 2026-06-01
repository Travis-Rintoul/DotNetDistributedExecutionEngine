using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Lifecycle;

public class MarkRunningCommandHandler : ICommandHandler<MarkWorkerRunningCommand, Result<Unit, MarkWorkerRunningError>>
{
    public Task<Result<Unit, MarkWorkerRunningError>> HandleAsync(MarkWorkerRunningCommand command, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}