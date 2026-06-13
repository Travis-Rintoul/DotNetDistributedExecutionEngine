namespace DistributedExecutionEngine.Application.Features.JobTypes.Queries;

public record JobTypeDto
{
    public string Code { get; init; }
    public bool IsEnabled { get; init; }
}