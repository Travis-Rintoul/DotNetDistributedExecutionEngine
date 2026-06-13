using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Jobs.Persistence;

namespace DistributedExecutionEngine.Application.Features.Jobs.Queries;

public class GetJobsQueryHandler(IJobQueries jobQueries) : IQueryHandler<GetJobsQuery, IReadOnlyList<JobDto>>
{
    public async Task<IReadOnlyList<JobDto>> HandleAsync(GetJobsQuery query, CancellationToken cancellationToken)
        => await jobQueries.GetJobsAsync(cancellationToken: cancellationToken);
}