using System.Windows.Input;
using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Health;

public class RecordWorkerHeartbeatCommandHandler : ICommandHandler<RecordWorkerHeartbeatCommand, Result<Unit, WorkerHeartbeatError>>
{
    public Task<Result<Unit, WorkerHeartbeatError>> HandleAsync(RecordWorkerHeartbeatCommand command)
    {
        throw new NotImplementedException();
    }
}