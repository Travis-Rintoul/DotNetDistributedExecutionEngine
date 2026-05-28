namespace DistributedExecutionEngine.Domain.Aggregates.Workers;

public readonly record struct WorkerId(Guid Value)
{
    public static WorkerId New() => new(Guid.NewGuid());
    
    public static WorkerId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("WorkerId cannot be empty.", nameof(value));

        return new WorkerId(value);
    }
    
    public bool IsEmpty => Value == Guid.Empty;
    
    public override string ToString() => Value.ToString();
}