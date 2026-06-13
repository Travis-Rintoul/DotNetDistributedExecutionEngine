using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Abstractions.Persistence;

namespace DistributedExecutionEngine.Application.Features.Workers.Queries;

public class GetWorkersQueryHandler(IWorkerQueries workerQueries): IQueryHandler<GetWorkersQuery, IReadOnlyList<WorkerDto>>
{
    public Task<IReadOnlyList<WorkerDto>> HandleAsync(GetWorkersQuery query, CancellationToken cancellationToken)
        => workerQueries.GetWorkersAsync(cancellationToken);
}