using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Abstractions.Persistence;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Jobs;

internal static class JobMapper
{
    public static Result<Job, MappingError> ToDomain(JobRecord persistence)
    {
        return Result<Job, MappingError>.Success(new Job());
    }

    public static JobRecord ToPersistence(Job domain)
    {
        return new JobRecord();
    }

    public static void ApplyToRecord(Job domain, JobRecord record)
    {
        throw new NotImplementedException();
    }
}