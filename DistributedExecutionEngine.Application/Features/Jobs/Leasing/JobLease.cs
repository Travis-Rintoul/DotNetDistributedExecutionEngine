using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.Workers;

namespace DistributedExecutionEngine.Application.Features.Jobs.Leasing;

public sealed record JobLease
{
    public JobId JobId { get; }
    public WorkerId WorkerId { get; }
    public DateTimeOffset ExpiresAt { get; }

    public bool IsExpired(IClock clock)
    {
        return clock.UtcNow >= ExpiresAt;
    }
}