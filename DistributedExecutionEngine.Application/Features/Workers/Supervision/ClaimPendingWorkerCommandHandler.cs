using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Workers.Lifecycle;
using DistributedExecutionEngine.Application.Features.Workers.Persistence;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Supervision;

public class ClaimPendingWorkerCommandHandler(
    IClock clock,
    IWorkerPoolStore store
) : ICommandHandler<ClaimPendingWorkerCommand, Result<Option<WorkerId>, string>>
{
    public async Task<Result<Option<WorkerId>, string>> HandleAsync(ClaimPendingWorkerCommand command, CancellationToken token = default)
    {
        return await store.ClaimNextPendingWorker(clock.UtcNow, command.supervisorId, token);
    }
}