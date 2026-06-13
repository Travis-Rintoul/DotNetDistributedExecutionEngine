using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Abstractions;

public interface IStatusMapper<in TRecord, out TCode, TStatus>
{
    TCode ToCode(TStatus record);
    Result<TStatus, string> Map(TRecord record);
    void ApplyToRecord(TStatus status, TRecord record);
}