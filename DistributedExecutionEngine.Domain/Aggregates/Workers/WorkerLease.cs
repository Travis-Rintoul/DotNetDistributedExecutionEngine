using DistributedExecutionEngine.Domain.Aggregates.Supervisor;

namespace DistributedExecutionEngine.Domain.Aggregates.Workers;

public abstract record WorkerLease
{
    public sealed record Available : WorkerLease;
    public sealed record Leased(SupervisorId SupervisorId, DateTimeOffset ClaimedUtc, DateTimeOffset ClaimExpires) : WorkerLease;
}