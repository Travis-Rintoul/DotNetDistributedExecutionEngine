namespace DistributedExecutionEngine.Infrastructure.Persistence.Mapping;

public interface IPersistenceShapeField<in TRecord>
{
    string Name { get; }

    bool IsDefault(TRecord record);

    void Reset(TRecord record);
}
