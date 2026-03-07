using DistributedExecutionEngine.Domain.Entities;
using DistributedExecutionEngine.Domain.Repositories;
using DistributedExecutionEngine.Infrastructure.Persistence;

namespace DistributedExecutionEngine.Infrastructure.Repositories;

public sealed class WorkerRepository(OrchestratorDbContext context) : IWorkerRepository
{
    public async Task<int> RegisterWorkerAsync(Worker worker)
    {
        await context.Workers.AddAsync(worker);
        await context.SaveChangesAsync();
        return worker.Id;
    }

    public async Task UpdateHeartbeat(Worker worker)
    {
        worker.LastHeartbeatAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }

    public Task<int> Count()
        => Task.FromResult(context.Workers.Count());
}