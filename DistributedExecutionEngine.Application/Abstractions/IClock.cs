namespace DistributedExecutionEngine.Application.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}