using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.JobTypes;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Jobs;

public sealed class JobRecord
{
    public long Id { get; set; }
    public Guid JobId { get; set; }
    public JobStatusCode Status { get; set; }
    public string? PayloadJson { get; set; }
    public string JobTypeCode { get; set; }
    public DateTimeOffset CreatedUtc { get; private set; }
    public DateTimeOffset? StartedUtc { get; private set; }
    public DateTimeOffset? CompletedUtc { get; private set; }
    public DateTimeOffset? LeasedUtc { get; set; }
    public int? AssignedWorkerId { get; set; }
    public int AttemptsCount { get; set; }
    public int MaxAttemptsCount { get; set; }
}