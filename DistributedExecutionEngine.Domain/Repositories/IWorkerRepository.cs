using DistributedExecutionEngine.Domain.Entities;

namespace DistributedExecutionEngine.Domain.Repositories;

public interface IWorkerRepository
{
    public Task<int> RegisterWorkerAsync(Worker worker);
    public Task<Worker?> ClaimPendingWorkerAsync();
    public Task<Worker?> GetByIdAsync(int id);
    public Task SaveAsync(Worker worker);
    public Task<int> CountAsync();
}