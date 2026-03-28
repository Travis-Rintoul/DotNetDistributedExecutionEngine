using DistributedExecutionEngine.Domain.Repositories;
using DistributedExecutionEngine.Library.Application.Worker;

namespace DistributedExecutionEngine.Domain.Entities;

public sealed class Worker
{
    public int Id { get; set; }
    public string Hostname { get; set; } = null!;
    public WorkerStatus Status { get; set; }
    public int MaxConcurrency { get; set; }
    public DateTime LastHeartbeatAt { get; set; }

    public static Worker Create(string hostname)
    {
        return new Worker
        {
            Id = 0,
            Hostname = hostname,
            Status = WorkerStatus.Pending,
            MaxConcurrency = Environment.ProcessorCount,
            LastHeartbeatAt = DateTime.MinValue
        };
    }
}