using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Lifecycle;

public class MarkWorkerPendingCommandHandler : ICommandHandler<MarkWorkerPendingCommand, Result<Unit, MarkWorkerPendingError>>
{
    public Task<Result<Unit, MarkWorkerPendingError>> HandleAsync(MarkWorkerPendingCommand command, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}