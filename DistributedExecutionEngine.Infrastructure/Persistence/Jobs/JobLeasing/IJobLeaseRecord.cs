using DistributedExecutionEngine.Domain.Aggregates.Jobs;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobLeasing;

public interface IJobLeaseRecord
{
    public JobLeaseStatusCode LeaseStatusCode { get; set; }
    public DateTimeOffset? LeasedUtc { get; set; }
    public Guid? AssignedWorkerId { get; set; }
}