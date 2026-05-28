namespace DistributedExecutionEngine.Application.Features.Jobs.Persistence;

public interface IJobQueries
{
    public Task<int> CountPendingAsync(CancellationToken cancellationToken = default);
}