namespace DistributedExecutionEngine.Application.Abstractions.Persistence;

public interface IWorkerQueries
{
    public Task<int> CountAsync(CancellationToken cancellationToken = default);
    
}