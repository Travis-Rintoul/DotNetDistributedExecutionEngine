using DistributedExecutionEngine.Domain.Aggregates.Supervisor;

namespace DistributedExecutionEngine.Domain.Aggregates.Workers;

public abstract record WorkerLease
{
    public sealed record Unclaimed : WorkerLease;
    public sealed record Claimed(SupervisorId SupervisorId, DateTimeOffset ClaimedUtc) : WorkerLease;
}