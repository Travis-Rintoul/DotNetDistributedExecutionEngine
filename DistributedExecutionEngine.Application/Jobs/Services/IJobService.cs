using System.Threading;
using System.Threading.Tasks;
using DistributedExecutionEngine.Domain.Entities;

namespace DistributedExecutionEngine.Application.Jobs.Services;

public interface IJobService
{
    Task<Job?> LeaseJob(int workerId);
    Task ScheduleJob(Job job);
    Task<int> PendingJobsCountAsync();
    Task<bool> ProcessNextJobAsync(int workerId, CancellationToken token);
}