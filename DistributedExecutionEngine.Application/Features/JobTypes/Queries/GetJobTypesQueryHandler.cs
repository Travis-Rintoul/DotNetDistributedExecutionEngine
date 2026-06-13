using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.JobTypes.Persistence;

namespace DistributedExecutionEngine.Application.Features.JobTypes.Queries;

public class GetJobTypesQueryHandler(IJobTypesQueries jobTypesQueries) : IQueryHandler<GetJobTypesQuery, IReadOnlyList<JobTypeDto>>
{
    public async Task<IReadOnlyList<JobTypeDto>> HandleAsync(GetJobTypesQuery query, CancellationToken cancellationToken)
        => await jobTypesQueries.GetJobTypesAsync(cancellationToken);
}