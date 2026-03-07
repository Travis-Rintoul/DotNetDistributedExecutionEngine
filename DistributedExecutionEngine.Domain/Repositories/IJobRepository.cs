using DistributedExecutionEngine.Domain.Entities;

namespace DistributedExecutionEngine.Domain.Repositories;

public interface IJobRepository
{
    Task<Job?> LeaseNextPendingJobAsync(int workerId);
    Task<IEnumerable<Job>> GetPendingJobsAsync();
    Task ScheduleJobAsync(Job job);
    Task<int> PendingJobsCountAsync();
    
}