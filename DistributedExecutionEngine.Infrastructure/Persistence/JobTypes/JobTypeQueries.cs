using DistributedExecutionEngine.Application.Features.JobTypes.Persistence;
using DistributedExecutionEngine.Application.Features.JobTypes.Queries;
using Microsoft.EntityFrameworkCore;

namespace DistributedExecutionEngine.Infrastructure.Persistence.JobTypes;

public class JobTypeQueries(DistributedExecutionDbContext dbContext): IJobTypesQueries
{
    public async Task<IReadOnlyList<JobTypeDto>> GetJobTypesAsync(CancellationToken cancellationToken) =>
        await dbContext.JobTypes.Select(type => new JobTypeDto
            {
                Code = type.Code,
                IsEnabled = type.IsEnabled
            })
            .ToListAsync(cancellationToken);
}