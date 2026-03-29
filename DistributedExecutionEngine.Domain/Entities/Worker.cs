using DistributedExecutionEngine.Domain.Enums;
using DistributedExecutionEngine.Domain.Repositories;

namespace DistributedExecutionEngine.Domain.Entities;

public sealed class Worker
{
    public int Id { get; set; }
    public Guid InstanceId { get; set; }
    public string Hostname { get; set; } = null!;
    public WorkerStatus Status { get; set; }
    public int MaxConcurrency { get; set; }
    public DateTime LastHeartbeatAt { get; set; }

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
}