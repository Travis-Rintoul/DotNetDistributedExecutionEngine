using DistributedExecutionEngine.Application.Abstractions.Persistence;
using DistributedExecutionEngine.Application.Features.Workers.Queries;
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
}