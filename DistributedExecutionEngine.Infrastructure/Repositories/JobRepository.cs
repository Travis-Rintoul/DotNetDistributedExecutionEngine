using DistributedExecutionEngine.Domain.Entities;
using DistributedExecutionEngine.Domain.Enums;
using DistributedExecutionEngine.Domain.Repositories;
using DistributedExecutionEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DistributedExecutionEngine.Infrastructure.Repositories;

public sealed class JobRepository(OrchestratorDbContext context) : IJobRepository
{
    public async Task<Job?> LeaseNextPendingJobAsync(int workerId)
    {
        var job = await context.Jobs
            .AsNoTracking()
            .Where(j => j.Status == JobStatus.Pending && j.AssignedWorkerId == null)
            .OrderBy(j => j.CreatedUtc)
            .FirstOrDefaultAsync();
        
        if (job == null) 
            return null;
        
        job.Status = JobStatus.Running;
        job.AssignedWorkerId = workerId;
        job.LeasedUtc = DateTime.UtcNow;
        
        context.Jobs.Update(job);

        await context.SaveChangesAsync();
        
        return job;
    }

    public async Task<IEnumerable<Job>> GetPendingJobsAsync()
        => await context.Jobs
            .AsNoTracking()
            .Where(job => job.Status == JobStatus.Pending)
            .ToListAsync();

    public async Task ScheduleJobAsync(Job job)
    {
        job.Status = JobStatus.Pending;
        job.CreatedUtc = DateTime.UtcNow;
        
        await context.Jobs.AddAsync(job);
        await context.SaveChangesAsync();
    }

    public async Task<int> PendingJobsCountAsync()
        => (await this.GetPendingJobsAsync()).Count();
}