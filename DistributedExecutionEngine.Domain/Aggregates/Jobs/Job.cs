using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Domain.Aggregates.Jobs;

public sealed class Job
{
    public JobId JobId { get; private init; }
    public DateTimeOffset CreatedUtc { get; private init; }

    public JobStatus Status { get; private set; }
    public JobLease Lease { get; private set; }
    public string JobType { get; private init; } = null!;
    public string? PayloadJson { get; private init; }
    public int AttemptsCount { get; private set; }
    public int MaxAttemptsCount { get; private init; }
    public Option<WorkerId> AssignedWorkerId { get; private init; }

    public static Job Create(string jobType, string? payloadJson = "GENERIC")
    {
        return new Job
        {
            JobId = JobId.New(),
            JobType = jobType,
            PayloadJson = payloadJson,
            CreatedUtc = DateTime.UtcNow,
            AssignedWorkerId = Option<WorkerId>.None,
            Status = new JobStatus.Pending(),
            Lease = new JobLease.Available(),
        };
    }
    
    public static Job Rehydrate(
        Guid jobId,
        string jobType,
        string? payloadJson,
        JobStatus status,
        JobLease lease,
        DateTimeOffset createdUtc,
        int attemptsCount,
        int maxAttemptsCount, 
        Option<WorkerId> assignedWorkerId)
    {
        return new Job
        {
            JobId = JobId.From(jobId),
            JobType = jobType,
            PayloadJson = payloadJson,
            Status = status,
            Lease = lease,
            CreatedUtc = createdUtc,
            AttemptsCount = attemptsCount,
            MaxAttemptsCount = maxAttemptsCount,
            AssignedWorkerId = Option<WorkerId>.None,
        };
    }

    public void MarkRunning()
    {
        Status = new JobStatus.Running(DateTime.UtcNow);
    }
    
    public void MarkPending()
    {
        Status = new JobStatus.Pending();
    }

    public void LeaseJob(DateTimeOffset leasedUtc, WorkerId workerId)
    {
        Lease = new JobLease.Leased(leasedUtc, workerId);
    }
    
    public void FreeJob()
    {
        Lease = new JobLease.Available();
    }
}