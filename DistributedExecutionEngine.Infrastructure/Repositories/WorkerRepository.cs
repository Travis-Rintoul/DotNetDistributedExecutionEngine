using DistributedExecutionEngine.Domain.Entities;
using DistributedExecutionEngine.Domain.Repositories;
using DistributedExecutionEngine.Infrastructure.Persistence;
using DistributedExecutionEngine.Library.Application.Worker;
using Microsoft.EntityFrameworkCore;

namespace DistributedExecutionEngine.Infrastructure.Repositories;

public sealed class WorkerRepository(OrchestratorDbContext context) : IWorkerRepository
{
    public async Task<int> RegisterWorkerAsync(Worker worker)
    {
        await context.Workers.AddAsync(worker);
        await context.SaveChangesAsync();
        return worker.Id;
    }

    public async Task<Worker?> ClaimPendingWorkerAsync()
    {
        const int starting = (int)WorkerStatus.Starting;
        const int pending = (int)WorkerStatus.Pending;

        var results = await context.Workers
            .FromSqlRaw("""
                UPDATE "Workers"
                SET "Status" = {0}
                WHERE "Id" = (
                    SELECT "Id" FROM "Workers"
                    WHERE "Status" = {1}
                    ORDER BY "Id"
                    LIMIT 1
                    FOR UPDATE SKIP LOCKED
                )
                RETURNING *;
            """, starting, pending)
            .AsNoTracking()
            .ToListAsync();

        return results.SingleOrDefault();
    }

    public async Task UpdateHeartbeat(Worker worker)
    {
        worker.LastHeartbeatAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    public Task<int> Count()
        => Task.FromResult(context.Workers.Count());
}