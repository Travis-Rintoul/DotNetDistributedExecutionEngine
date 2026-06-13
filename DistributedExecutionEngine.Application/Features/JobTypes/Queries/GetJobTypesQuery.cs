using DistributedExecutionEngine.Application.Abstractions.Messaging;

namespace DistributedExecutionEngine.Application.Features.JobTypes.Queries;

public class GetJobTypesQuery : IQuery<IReadOnlyList<JobTypeDto>> { } 