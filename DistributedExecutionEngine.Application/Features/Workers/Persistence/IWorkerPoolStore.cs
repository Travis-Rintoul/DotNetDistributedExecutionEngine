using DistributedExecutionEngine.Application.Features.Workers.Supervision;
using DistributedExecutionEngine.Domain.Aggregates.Supervisor;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Persistence;

public interface IWorkerPoolStore
{
    Task<Result<IReadOnlyList<WorkerId>, string>> ClaimPendingWorkersForStartup(SupervisorId supervisorId, int limit, CancellationToken cancellationToken);
    
    Task<Result<IReadOnlyList<WorkerId>, string>> ClaimWorkersForSupervision(SupervisorId supervisorId, int limit, CancellationToken cancellationToken);

    Task<Result<Option<WorkerId>, string>> TryProvisionWorkerForPendingJobsAsync(CancellationToken cancellationToken);
}