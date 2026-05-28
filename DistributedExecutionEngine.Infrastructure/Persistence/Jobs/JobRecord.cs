namespace DistributedExecutionEngine.Infrastructure.Persistence.Jobs;

public sealed class JobRecord
{
    public long Id { get; set; }
    public Guid JobId { get; set; }
    public string? PayloadJson { get; set; }
    public Domain.Aggregates.JobTypes.JobType JobType { get; set; }
    public DateTime CreatedUtc { get; private set; }
    public DateTime? StartedUtc { get; private set; }
    public DateTime? CompletedUtc { get; private set; }
    public DateTime? LeasedUtc { get; set; }
    public int? AssignedWorkerId { get; set; }
    public int AttemptsCount { get; set; }
    public int MaxAttemptsCount { get; set; }
}