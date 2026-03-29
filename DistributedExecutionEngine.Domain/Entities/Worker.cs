using DistributedExecutionEngine.Domain.Enums;
using DistributedExecutionEngine.Domain.Repositories;

namespace DistributedExecutionEngine.Domain.Entities;

public sealed class Worker
{
    public int Id { get; set; }
    public Guid InstanceId { get; set; }
    public string Hostname { get; set; } = null!;
    public WorkerStatus Status { get; private set; }
    public int MaxConcurrency { get; set; }
    public DateTime LastHeartbeatAt { get; private set; }

    public static Worker Create(string hostname = "worker")
    {
        return new Worker
        {
            Id = 0,
            InstanceId = Guid.Empty,
            Hostname = hostname,
            Status = WorkerStatus.Pending,
            MaxConcurrency = Environment.ProcessorCount,
            LastHeartbeatAt = DateTime.MinValue
        };
    }

    public void MarkRunning()
    {
        if (Status != WorkerStatus.Starting)
        {
            throw new InvalidOperationException(
                $"Invalid transition: {Status} -> Running");
        }
        
        Status = WorkerStatus.Running;
    }
    
    public void UpdateHeartbeat()
    {
        if (Status != WorkerStatus.Running)
        {
            throw new InvalidOperationException(
                "Heartbeat only valid for running workers");
        }
        
        LastHeartbeatAt = DateTime.UtcNow;
    }
}