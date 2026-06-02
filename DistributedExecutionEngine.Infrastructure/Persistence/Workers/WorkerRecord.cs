using DistributedExecutionEngine.Application.Features.Workers.Supervision;
using DistributedExecutionEngine.Domain.Aggregates.Workers;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers;

public sealed record WorkerRecord
{
    public long Id { get; init; }
    public Guid WorkerId { get; init; }
    public string Hostname { get; init; } = string.Empty;
    public WorkerStatusCode Status { get; init; }
    public int MaxConcurrency { get; init; }
    
    public DateTimeOffset CreatedUtc { get; init; }
    public DateTimeOffset? LastHeartbeatAt { get; init; }
    
    public SupervisorId? SupervisorId { get; init; }
    public DateTimeOffset? ClaimedUtc { get; init; }
}