using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Workers.Persistence;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Supervision;

public class ClaimPendingWorkersForSupervisorCommandHandler(IClock clock, IWorkerPoolStore store) : ICommandHandler<ClaimPendingWorkersForSupervisorCommand, Result<IReadOnlyList<WorkerId>, string>>
{
    public async Task<Result<IReadOnlyList<WorkerId>, string>> HandleAsync(ClaimPendingWorkersForSupervisorCommand command, CancellationToken token = default) =>
        await store.ClaimPendingWorkersForStartup(command.SupervisorId, 10, token);
}