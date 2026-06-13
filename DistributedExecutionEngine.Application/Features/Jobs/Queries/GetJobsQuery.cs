using DistributedExecutionEngine.Application.Abstractions.Messaging;

namespace DistributedExecutionEngine.Application.Features.Jobs.Queries;

public class GetJobsQuery : IQuery<IReadOnlyList<JobDto>> { }