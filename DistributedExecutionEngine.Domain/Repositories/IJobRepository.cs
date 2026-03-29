using DistributedExecutionEngine.Domain.Entities;

namespace DistributedExecutionEngine.Domain.Repositories;

public interface IJobRepository
{
    Task<Job?> ClaimNextPendingJobAsync();
    Task<Job?> GetByIdAsync(int id);
    Task AddAsync(Job job);
    Task SaveAsync(Job job);
    Task<int> CountPendingAsync();
}