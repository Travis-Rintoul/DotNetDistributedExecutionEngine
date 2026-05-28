using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Abstractions.Persistence;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers;

internal static class WorkerMapper
{
    public static Result<Worker, MappingError> ToDomain(WorkerRecord persistence)
    {
        throw new NotImplementedException();
    }

    public static WorkerRecord ToPersistence(Worker domain)
    {
        throw new NotImplementedException();
    }

    public static void ApplyToRecord(WorkerRecord domain, WorkerRecord record)
    {
        throw new NotImplementedException();
    }
}