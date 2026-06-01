using DistributedExecutionEngine.Application.Features.Workers.Supervision;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Persistence;

public interface IWorkerPoolStore
{
    Task<Result<Option<WorkerId>, string>> ClaimNextPendingWorker(DateTimeOffset nowUtc, SupervisorId supervisorId,
        CancellationToken cancellationToken);

    Task<Result<Option<WorkerId>, string>> ReconcileAsync(DateTimeOffset nowUtc, CancellationToken cancellationToken);
}