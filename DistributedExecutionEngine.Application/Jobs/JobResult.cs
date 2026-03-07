namespace DistributedExecutionEngine.Application.Jobs;

public sealed record JobResult
{
    public string Message { get; init; } = string.Empty;
}