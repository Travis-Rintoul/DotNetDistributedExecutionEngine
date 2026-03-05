using DistributedExecutionEngine.Domain.Entities;

namespace DistributedExecutionEngine.Application.Abstractions.Repositories;

public interface IJobRepository
{
    Task<Job?> GetAsync(Guid id);
    Task SaveAsync(Job job);
}