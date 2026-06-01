namespace DistributedExecutionEngine.Application.Abstractions;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTime.UtcNow;
}