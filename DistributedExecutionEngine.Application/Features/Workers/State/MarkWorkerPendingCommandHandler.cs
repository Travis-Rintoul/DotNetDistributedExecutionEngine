using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.State;

public class MarkWorkerPendingCommandHandler : ICommandHandler<MarkWorkerPendingCommand, Result<Unit, MarkWorkerPendingError>>
{
    public Task<Result<Unit, MarkWorkerPendingError>> HandleAsync(MarkWorkerPendingCommand command)
    {
        throw new NotImplementedException();
    }
}