using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.Workers;

namespace DistributedExecutionEngine.Application.Features.Jobs.Leasing;

public sealed record JobWorkerLease
{
    public JobId JobId { get; init; }
    public WorkerId WorkerId { get; init; }
    public DateTimeOffset LeasedAtUtc { get; init; }
    public DateTimeOffset ExpiresAtUtc { get; init; }

    public bool IsExpired(IClock clock)
        => clock.UtcNow >= ExpiresAtUtc;
}