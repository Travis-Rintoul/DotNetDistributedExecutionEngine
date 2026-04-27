using System.Threading;
using System.Threading.Tasks;
using DistributedExecutionEngine.Domain.Entities;
using DistributedExecutionEngine.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace DistributedExecutionEngine.Application.Jobs.Services;

public sealed class JobService(
    IJobRepository jobsRepository, 
    IJobExecutorService jobExecutorService, 
    ILogger<JobService> logger
) : IJobService
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

    public async Task<bool> ProcessNextJobAsync(int workerId, CancellationToken token)
    {
        var job = await LeaseJob(workerId);
        if (job is null)
        {
            return false;
        }
                
        logger.LogInformation($"Worker ({workerId}) found job: {job.Id}");
        
        var result = await jobExecutorService.ExecuteJob(job);
        
        
        
        throw new System.NotImplementedException();
    }
}