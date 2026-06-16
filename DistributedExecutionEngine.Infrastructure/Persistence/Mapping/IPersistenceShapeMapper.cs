using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Mapping;

public interface IPersistenceShapeMapper<in TRecord, out TCode, TShape> where TCode : struct, Enum
{
    TCode ToCode(TShape shape);
    Result<TShape, string> ToDomain(TRecord record);
}