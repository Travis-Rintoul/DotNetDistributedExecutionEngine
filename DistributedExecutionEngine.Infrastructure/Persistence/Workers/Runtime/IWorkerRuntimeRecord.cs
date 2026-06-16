using DistributedExecutionEngine.Domain.Aggregates.Workers;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers.Runtime;

public interface IWorkerRuntimeRecord
{
    public WorkerStatusCode StatusCode { get; set; }
    
    public int? ProcessId { get; set; }
    public string? Hostname { get; set; }
    public string? MachineName { get; set; }
    public DateTimeOffset? ProcessStartedUtc { get; set; }
    public DateTimeOffset? RunningSinceUtc { get; set; }
    public DateTimeOffset? LastHeartbeatUtc { get; set; }
    public int StartupAttemptCount { get; set; }
    public int MaxStartupAttemptCount { get; set; }
}