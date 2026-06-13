using DistributedExecutionEngine.Application.Features.Jobs.Queries;

namespace DistributedExecutionEngine.Application.Features.Jobs.Persistence;

public interface IJobQueries
{
    public Task<int> CountPendingAsync(CancellationToken cancellationToken = default);
    public Task<IReadOnlyList<JobDto>> GetJobsAsync(CancellationToken cancellationToken = default);
}