using DistributedExecutionEngine.Domain.Aggregates.Workers;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers;

public class WorkerRecord
{
    public long Id { get; set; }
    public Guid WorkerId { get; set; }
    public string Hostname { get; set; } = string.Empty;
    public WorkerStatusCode Status { get; set; }
    public int MaxConcurrency { get; set; }
    public DateTime LastHeartbeatAt { get; private set; }
}