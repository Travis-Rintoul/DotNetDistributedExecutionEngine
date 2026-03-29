using System.Threading.Tasks;
using DistributedExecutionEngine.Domain.Entities;
using DistributedExecutionEngine.Domain.Repositories;

namespace DistributedExecutionEngine.Application.Jobs.Services;

public sealed class JobService(IJobRepository jobsRepository) : IJobService
{
    public async Task<Job?> LeaseJob(int workerId)
    {
        var job = await jobsRepository.ClaimNextPendingJobAsync();
        if (job is null)
        {
            return null;
        }
        
        job.AssignWorker(workerId);
        job.MarkRunning();
        
        await jobsRepository.SaveAsync(job);
        
        return job;
    }

    public async Task ScheduleJob(Job job)
    {
        job.MarkPending();
        await jobsRepository.AddAsync(job);
    }

    public async Task<int> PendingJobsCountAsync()
        => await jobsRepository.CountPendingAsync();
}