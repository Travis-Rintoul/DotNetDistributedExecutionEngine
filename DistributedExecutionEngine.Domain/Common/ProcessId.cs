namespace DistributedExecutionEngine.Domain.Common;

public record struct ProcessId(int Value)
{
    public static ProcessId From(int value) => new(value);
}