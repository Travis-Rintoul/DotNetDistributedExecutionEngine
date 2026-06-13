using DistributedExecutionEngine.Application.Abstractions.Messaging;

namespace DistributedExecutionEngine.Application.Features.Workers.Queries;

public class GetWorkersQuery : IQuery<IReadOnlyList<WorkerDto>> { }