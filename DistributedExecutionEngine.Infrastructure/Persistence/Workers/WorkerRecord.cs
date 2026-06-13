using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerLeases;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerStatuses;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers;

public sealed record WorkerRecord : IWorkerStatusRecord, IWorkerLeaseRecord
{
    public long Id { get; set; }
    public Guid WorkerId { get; set; }
    public string Hostname { get; set; } = string.Empty;
    public DateTimeOffset CreatedUtc { get; set; }
    public DateTimeOffset? LastHeartbeatAtUtc { get; set; }
    public int MaxConcurrency { get; set; }
    
    // Status Fields
    public WorkerStatusCode StatusCode { get; set; }
    public DateTimeOffset? StartedUtc { get; set; }
    public DateTimeOffset? CompletedUtc { get; set; }
    public DateTimeOffset? FailedUtc { get; set; }
    public string? FailureReason { get; set; }
    public DateTimeOffset? CanceledUtc { get; set; }
    public DateTimeOffset? StoppedUtc { get; set; }
    public string? CancellationReason { get; set; }
    
    // Lease Fields
    public WorkerLeaseStatusCode LeaseStatusCode { get; set; }
    public int? SupervisionLeasedBy { get; set; }
    public DateTimeOffset? SupervisionLeasedUtc { get; set; }
    public DateTimeOffset? SupervisionLeaseExpiresUtc { get; set; }
}