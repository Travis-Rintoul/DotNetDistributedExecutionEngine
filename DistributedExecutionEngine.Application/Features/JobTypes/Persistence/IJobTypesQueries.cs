using DistributedExecutionEngine.Application.Features.JobTypes.Queries;

namespace DistributedExecutionEngine.Application.Features.JobTypes.Persistence;

public interface IJobTypesQueries
{
    public Task<IReadOnlyList<JobTypeDto>> GetJobTypesAsync(CancellationToken cancellationToken);    
}