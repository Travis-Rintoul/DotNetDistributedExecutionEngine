using System.Threading.Tasks;
using DistributedExecutionEngine.Domain.Entities;
using DistributedExecutionEngine.Domain.Repositories;

namespace DistributedExecutionEngine.Application.Jobs.Services;

public sealed class JobService(IJobRepository jobsRepository) : IJobService
{
    public async Task<Job?> LeaseJob(int workerId)
        => await jobsRepository.LeaseNextPendingJobAsync(workerId);

    public Task ScheduleJob(Job job)
        => jobsRepository.ScheduleJobAsync(job);
}