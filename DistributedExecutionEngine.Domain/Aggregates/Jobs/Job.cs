using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Domain.Aggregates.Jobs;

public abstract record JobStatus
{
    public sealed record Pending() : JobStatus;

    public sealed record Running(DateTime StartedUtc) : JobStatus;

    public sealed record Completed : JobStatus
    {
        public DateTime StartedUtc { get; init; }
        public DateTime CompletedUtc { get; init; }
    }

    public sealed record Failed : JobStatus
    {
        public DateTime FailedUtc { get; init; }
        public string Reason { get; init; } = string.Empty;
    }
}

public sealed class Job
{
    public JobId JobId { get; private init; }
    public DateTime CreatedUtc { get; private set; }
    public JobStatus Status { get; private set; }
    public Option<DateTime> LeasedUtc { get; private set; }
    
    public string? PayloadJson { get; set; }
    public string? JobType { get; set; } = null!;

    public int? AssignedWorkerId { get; set; }
    public int AttemptsCount { get; set; }
    public int MaxAttemptsCount { get; set; }

    public static Job Create(string jobType, string? payloadJson)
    {
        return new Job
        {
            JobId = new JobId(),
            Status = new JobStatus.Pending(),
            PayloadJson = payloadJson,
            JobType = jobType,
            CreatedUtc = DateTime.UtcNow,
        };
    }

    public void AssignWorker(int workerId)
    {
        AssignedWorkerId = workerId;
        LeasedUtc = Option<DateTime>.Some(DateTime.UtcNow);
    }
    
    public void MarkRunning()
    {
        Status = new JobStatus.Running(DateTime.UtcNow);
    }
    
    public void MarkPending()
    {
        Status = new JobStatus.Pending();
    }
}