namespace DistributedExecutionEngine.Domain.Aggregates.Supervisor;

public record struct SupervisorId(int Value)
{
    public static SupervisorId From(int value)
        => new(value);
}