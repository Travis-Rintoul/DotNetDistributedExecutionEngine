using DistributedExecutionEngine.Application.Features.Workers.Queries;

namespace DistributedExecutionEngine.Application.Abstractions.Persistence;

public interface IWorkerQueries
{
    public Task<int> CountAsync(CancellationToken cancellationToken = default);
    public Task<IReadOnlyList<WorkerDto>> GetWorkersAsync(CancellationToken cancellationToken = default); 
}