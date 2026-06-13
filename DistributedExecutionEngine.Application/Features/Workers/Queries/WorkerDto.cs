using DistributedExecutionEngine.Domain.Aggregates.Workers;

namespace DistributedExecutionEngine.Application.Features.Workers.Queries;

public record WorkerDto
{
    public Guid WorkerId { get; init; }
    public string Hostname { get; init; }
}