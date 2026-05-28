namespace DistributedExecutionEngine.Domain.Aggregates.Jobs;

public enum JobStatusCode
{
    Pending = 0,
    Running = 1,
    Failed = 2,
    Succeeded = 4,
}