using DistributedExecutionEngine.Application.Features.Jobs.Persistence;
using DistributedExecutionEngine.Application.Features.Jobs.Queries;
using Microsoft.EntityFrameworkCore;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Jobs;

public class JobQueries(DistributedExecutionDbContext dbContext) : IJobQueries
{
    public async Task<IReadOnlyList<JobDto>> GetJobsAsync(CancellationToken cancellationToken)
        => await dbContext.Jobs
            .AsNoTracking()
            .Select(job => new JobDto
            {
                CreatedAt = job.CreatedUtc,
                JobId = job.JobId,
                JobStatus = job.StatusCode
            })
            .ToListAsync(cancellationToken);

    public async Task<int> CountPendingAsync(CancellationToken cancellationToken = default)
        =>  await dbContext.Jobs.CountAsync(cancellationToken);
}