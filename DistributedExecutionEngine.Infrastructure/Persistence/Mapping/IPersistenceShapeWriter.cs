using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Mapping;

public interface IPersistenceShapeWriter<in TRecord, in TShape>
{
    Result<Unit, string> ApplyToRecord(TShape shape, TRecord record);
}