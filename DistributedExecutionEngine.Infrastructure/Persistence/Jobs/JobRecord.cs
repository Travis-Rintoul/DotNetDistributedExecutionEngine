using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.JobTypes;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobLeasing;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobStatuses;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Jobs;

public sealed record JobRecord : IJobStatusRecord, IJobLeaseRecord
{
    // Main fields
    public long Id { get; set; }
    public Guid JobId { get; set; }
    public string? PayloadJson { get; set; }
    public string JobTypeCode { get; set; }
    public int AttemptsCount { get; set; }
    public int MaxAttemptsCount { get; set; }
    
    // Status fields
    public JobStatusCode StatusCode { get; set; }
    public DateTimeOffset CreatedUtc { get; set; }
    public DateTimeOffset? StartedUtc { get; set; }
    public DateTimeOffset? CompletedUtc { get; set; }
    public DateTimeOffset? FailedUtc { get; set; }
    public string? FailureReason { get; set; }
    
    // Lease fields
    public JobLeaseStatusCode LeaseStatusCode { get; set; }
    public DateTimeOffset? LeasedUtc { get; set; }
    public DateTimeOffset? LeaseExpirationUtc { get; set; }
    public Guid? AssignedWorkerId { get; set; }
    
}