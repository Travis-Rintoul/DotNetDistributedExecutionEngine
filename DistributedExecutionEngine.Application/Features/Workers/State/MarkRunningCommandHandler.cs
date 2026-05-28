using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.State;

public class MarkRunningCommandHandler : ICommandHandler<MarkWorkerRunningCommand, Result<Unit, MarkWorkerRunningError>>
{
    public Task<Result<Unit, MarkWorkerRunningError>> HandleAsync(MarkWorkerRunningCommand command)
    {
        throw new NotImplementedException();
    }
}