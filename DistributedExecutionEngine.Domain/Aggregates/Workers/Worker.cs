using DistributedExecutionEngine.Domain.Enums;

namespace DistributedExecutionEngine.Domain.Aggregates.Workers;

public sealed class Worker
{
    public int Id { get; set; }
    public WorkerId WorkerId { get; set; }
    public string Hostname { get; set; } = null!;
    public WorkerStatusCode Status { get; private set; }
    public int MaxConcurrency { get; set; }
    public DateTime LastHeartbeatAt { get; private set; }

    public static Worker Create(string hostname = "worker")
    {
        return new Worker
        {
            Id = 0,
            WorkerId = WorkerId.New(),
            Hostname = hostname,
            Status = WorkerStatusCode.Pending,
            MaxConcurrency = Environment.ProcessorCount,
            LastHeartbeatAt = DateTime.MinValue
        };
    }

    public void MarkRunning()
    {
        if (Status != WorkerStatusCode.Starting)
        {
            throw new InvalidOperationException(
                $"Invalid transition: {Status} -> Running");
        }
        
        Status = WorkerStatusCode.Running;
    }
    
    public void UpdateHeartbeat()
    {
        if (Status != WorkerStatusCode.Running)
        {
            throw new InvalidOperationException(
                "Heartbeat only valid for running workers");
        }
        
        LastHeartbeatAt = DateTime.UtcNow;
    }
}