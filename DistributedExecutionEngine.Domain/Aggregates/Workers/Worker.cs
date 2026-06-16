using DistributedExecutionEngine.Domain.Aggregates.Supervisor;
using DistributedExecutionEngine.Domain.Common;
using DistributedExecutionEngine.Domain.Enums;

namespace DistributedExecutionEngine.Domain.Aggregates.Workers;

public sealed class Worker
{
    public WorkerId WorkerId { get; private set; }
    public string Hostname { get; private set; } = null!;
    public DateTimeOffset CreatedUtc { get; private set; }
    public Option<DateTimeOffset> LastHeartbeatAt { get; private set; } = Option<DateTimeOffset>.None;
    public WorkerLease Lease { get; private set; } = new WorkerLease.Available();
    public WorkerStatus Status { get; private set; } = new WorkerStatus.Pending();
    public WorkerRuntime Runtime { get; private set; } = new WorkerRuntime.Pending();
    public int MaxConcurrency { get; private set; }

    public static Worker Create(string hostname = "worker")
    {
        return new Worker
        {
            WorkerId = WorkerId.New(),
            Hostname = hostname,
            Lease = new WorkerLease.Available(),
            Status = new WorkerStatus.Pending(),
            MaxConcurrency = Environment.ProcessorCount,
            LastHeartbeatAt = Option<DateTimeOffset>.None,
        };
    }

    public static Worker Rehydrate(
        Guid workerId,
        string hostname,
        DateTimeOffset createdUtc,
        WorkerStatus status,
        WorkerLease lease,
        WorkerRuntime runtime
    )
    {
        return new Worker
        {
            WorkerId = WorkerId.From(workerId),
            Hostname = hostname,
            Status = status,
            Lease = lease,
            Runtime = runtime,
            CreatedUtc = createdUtc
        };
    }

    public Result<Unit, string> MarkStarting()
    {
        if (Status is not WorkerStatus.Pending)
            return Result<Unit, string>.Failure($"Invalid state transition {Status} -> Starting");
        
        Status = new WorkerStatus.Starting();
        
        return Result<Unit, string>.Success(Unit.Value);
    }

    public Result<Unit, string> MarkRunning()
    {
        if (Status is not WorkerStatus.Starting)
            return Result<Unit, string>.Failure($"Invalid state transition {Status} -> Running");

        Status = new WorkerStatus.Running();
        
        return Result.Success<string>();
    }
    
    public Result<Unit, string> MarkCanceled(DateTimeOffset cancelTime, string reason)
    {
        Status = new WorkerStatus.Canceled(cancelTime, reason);
        return Result.Success<string>();
    }

    public void LeaseWorker(SupervisorId supervisorId, DateTimeOffset claimedUtc, DateTimeOffset expiresUtc)
    {
        Lease = new WorkerLease.Leased(supervisorId, claimedUtc, expiresUtc);
    }
    
    public void ReleaseWorker()
    {
        Lease = new WorkerLease.Available();
    }
    
    public void UpdateHeartbeat()
        => LastHeartbeatAt = Option<DateTimeOffset>.Some(DateTime.UtcNow);
}