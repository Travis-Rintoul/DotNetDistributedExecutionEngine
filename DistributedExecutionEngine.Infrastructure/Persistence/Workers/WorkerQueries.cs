using DistributedExecutionEngine.Application.Abstractions.Persistence;
using DistributedExecutionEngine.Application.Features.Workers.Queries;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using Microsoft.EntityFrameworkCore;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers;

public class WorkerQueries(DistributedExecutionDbContext dbContext) : IWorkerQueries
{
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await dbContext.Workers.CountAsync(cancellationToken);

    public async Task<IReadOnlyList<WorkerDto>> GetWorkersAsync(CancellationToken cancellationToken = default)
        => await dbContext.Workers
            .AsNoTracking()
            .Select(worker => new WorkerDto
            {
                Hostname = worker.Hostname,
                WorkerId = worker.WorkerId
            })
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<WorkerReconciliationSnapshotDto>> GetWorkersForReconciliationAsync(DateTimeOffset changedSinceUtc, CancellationToken cancellationToken = default)
        => await dbContext.Workers
            .AsNoTracking()
            .Where(worker =>
                worker.StatusCode == WorkerStatusCode.Pending ||
                worker.StatusCode == WorkerStatusCode.Starting ||
                worker.StatusCode == WorkerStatusCode.Running ||
                (
                    worker.UpdatedUtc >= changedSinceUtc ||
                    (
                        worker.StatusCode == WorkerStatusCode.Failed ||
                        worker.StatusCode == WorkerStatusCode.Canceled
                    )
                ))
            .Select(worker => new WorkerReconciliationSnapshotDto())// todo flesh out
            .ToListAsync(cancellationToken);
}