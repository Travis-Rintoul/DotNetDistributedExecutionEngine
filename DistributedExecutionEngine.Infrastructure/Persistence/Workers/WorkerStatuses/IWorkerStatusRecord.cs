using DistributedExecutionEngine.Domain.Aggregates.Workers;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerStatuses;

public interface IWorkerStatusRecord
{
    public WorkerStatusCode StatusCode { get; set; }
    public DateTimeOffset? StartedUtc { get; set; }
    public DateTimeOffset? StoppedUtc { get; set; }
    
    public DateTimeOffset? CompletedUtc { get; set; }
    
    public DateTimeOffset? FailedUtc { get; set; }
    public string? FailureReason { get; set; }
    
    public DateTimeOffset? CanceledUtc { get; set; }
    public string? CancellationReason { get; set; }
}

