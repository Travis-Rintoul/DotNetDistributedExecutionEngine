using DistributedExecutionEngine.Library.Domain.Enums;

namespace DistributedExecutionEngine.Domain.Entities;

public sealed class Job
{
    public int Id { get; set; }
    public Guid Guid { get; set; }
    public JobStatus Status { get; set; }
    public string? PayloadJson { get; set; }
    public string? JobType { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? StartedUtc { get; set; }
    public DateTime? CompletedUtc { get; set; }
    public DateTime? LeasedUtc { get; set; }
    public int? AssignedWorkerId { get; set; }
    
    public int AttemptsCount { get; set; }
    public int MaxAttemptsCount { get; set; }

    public static Job Create(string jobType, string? payloadJson)
    {
        return new Job
        {
            Guid = Guid.NewGuid(),
            Status = JobStatus.Pending,
            PayloadJson = payloadJson,
            JobType = jobType,
            CreatedUtc = DateTime.UtcNow,
        };
    }
}