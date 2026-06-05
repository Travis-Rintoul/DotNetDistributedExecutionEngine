using DistributedExecutionEngine.Domain.Aggregates.Jobs;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobStatuses;

public interface IJobStatusRecord
{
    public JobStatusCode StatusCode { get; set; }
    public DateTimeOffset CreatedUtc { get; set; }
    public DateTimeOffset? StartedUtc { get; set; }
    public DateTimeOffset? CompletedUtc { get; set; }
    
    public DateTimeOffset? FailedUtc { get; set; }
    public string? FailureReason { get; set; }
}