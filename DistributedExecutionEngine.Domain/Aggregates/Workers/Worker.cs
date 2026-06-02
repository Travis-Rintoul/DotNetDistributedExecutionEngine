using DistributedExecutionEngine.Domain.Common;
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

    public Result<Unit, string> MarkStarting()
    {
        if (Status != WorkerStatusCode.Pending)
            return Result<Unit, string>.Failure($"Invalid state transition {Status} -> Starting");
        
        Status = WorkerStatusCode.Starting;
        
        return Result<Unit, string>.Success(Unit.Value);
    }

    public Result<Unit, string> MarkRunning()
    {
        if (Status != WorkerStatusCode.Starting)
            return Result<Unit, string>.Failure($"Invalid state transition {Status} -> Running");
        
        Status = WorkerStatusCode.Running;
        
        return Result.Success<string>();
    }
    
    public void UpdateHeartbeat()
        => LastHeartbeatAt = DateTime.UtcNow;
}