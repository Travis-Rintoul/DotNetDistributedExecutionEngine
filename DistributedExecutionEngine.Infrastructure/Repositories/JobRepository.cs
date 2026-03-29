using DistributedExecutionEngine.Domain.Entities;
using DistributedExecutionEngine.Domain.Enums;
using DistributedExecutionEngine.Domain.Repositories;
using DistributedExecutionEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DistributedExecutionEngine.Infrastructure.Repositories;

public sealed class JobRepository(OrchestratorDbContext context) : IJobRepository
{
    public async Task<Job?> ClaimNextPendingJobAsync()
    {
        const int running = (int)JobStatus.Running;
        const int pending = (int)JobStatus.Pending;

        var result = await context.Jobs
            .FromSqlRaw("""
                UPDATE "Jobs"
                SET "Status" = {0}
                WHERE "Id" = (
                    SELECT "Id"
                    FROM "Jobs"
                    WHERE "Status" = {1}
                    ORDER BY "Id"
                    LIMIT 1
                    FOR UPDATE SKIP LOCKED
                )
                RETURNING *;
            """, running, pending)
            .AsNoTracking()
            .ToListAsync();

        return result.FirstOrDefault();
    }

    public async Task<Job?> GetByIdAsync(int id)
    {
        return await context.Jobs.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(Job job)
    {
        await context.Jobs.AddAsync(job);
        await context.SaveChangesAsync();
    }

    public async Task SaveAsync(Job job)
    {
        context.Jobs.Update(job);
        await context.SaveChangesAsync();
    }

    public async Task<int> CountPendingAsync()
        => await context.Jobs.CountAsync();
}