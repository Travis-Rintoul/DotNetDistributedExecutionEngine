namespace DistributedExecutionEngine.Domain.Aggregates.Jobs;

public readonly record struct JobId(Guid Value)
{
    public static JobId New() => new(Guid.NewGuid());
    
    public static JobId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("JobId cannot be empty.", nameof(value));

        return new JobId(value);
    }
    
    public bool IsEmpty => Value == Guid.Empty;
    
    public override string ToString() => Value.ToString();
}