using DistributedExecutionEngine.Domain.Entities;

namespace DistributedExecutionEngine.Domain.Repositories;

public interface IWorkerRepository
{
    public Task<int> RegisterWorkerAsync(Worker worker);
    public Task UpdateHeartbeat(Worker worker);
    public Task<int> Count();
}