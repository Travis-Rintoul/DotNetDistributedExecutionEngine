using DistributedExecutionEngine.Domain.Aggregates.Jobs;

namespace DistributedExecutionEngine.Application.Features.Jobs.Queries;

public record JobDto
{
    public Guid JobId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public JobStatusCode JobStatus { get; init; }
}