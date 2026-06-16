using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Workers.Persistence;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Reconciliation;

public class EnsureWorkerCapacityCommandHandler(
    IClock clock,
    IWorkerPoolStore store
) : ICommandHandler<EnsureWorkerCapacityCommand, Result<Option<WorkerId>, string>>
{
    public async Task<Result<Option<WorkerId>, string>> HandleAsync(EnsureWorkerCapacityCommand command, CancellationToken token = default)
        => await store.TryProvisionWorkerForPendingJobsAsync(token);
}