using DistributedExecutionEngine.Domain.Aggregates.Workers;

namespace DistributedExecutionEngine.Domain.Aggregates.Jobs;

public abstract record JobLease
{
    public sealed record Available() : JobLease;
    public sealed record Leased(DateTimeOffset LeasedUtc, WorkerId AssignedWorkerId) : JobLease; 
}