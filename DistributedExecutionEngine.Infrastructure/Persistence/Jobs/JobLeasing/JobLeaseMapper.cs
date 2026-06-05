using System.Diagnostics;
using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobLeasing;

public interface IJobLeaseMapper : IToDomainMapper<IJobLeaseRecord, JobLease>, IApplyToRecordMapper<JobLease, IJobLeaseRecord>;

public class JobLeaseMapper : IJobLeaseMapper
{
    public Result<JobLease, string> ToDomain(IJobLeaseRecord persistence)
    {
        return persistence.LeaseStatusCode switch
        {
            JobLeaseStatusCode.Leased => ToLeased(persistence),
            JobLeaseStatusCode.Available => Result.Success<JobLease, string>(new JobLease.Available()),
            _ => Result<JobLease, string>.Failure(
                $"Unknown worker lease status code: {persistence.LeaseStatusCode}.")
        };
    }

    public void ApplyToRecord(JobLease domain, IJobLeaseRecord record)
    {
        switch (domain)
        {
            case JobLease.Available:
                record.LeasedUtc = null;
                record.AssignedWorkerId = null;
                break;

            case JobLease.Leased leased:
                record.LeasedUtc = leased.LeasedUtc;
                record.AssignedWorkerId = leased.AssignedWorkerId.Value;
                break;

            default:
                throw new UnreachableException(
                    $"Unknown {nameof(JobLease)} type: {domain.GetType().Name}");
        }
    }

    private static Result<JobLease, string> ToLeased(IJobLeaseRecord persistence)
    {
        if (persistence.LeasedUtc is not { } leasedUtc)
        {
            return Result<JobLease, string>.Failure("LeasedUtc id is not assigned.");
        }
        
        if (persistence.AssignedWorkerId is not { } workerId)
        {
            return Result<JobLease, string>.Failure("Worker id is not assigned.");
        }

        return Result.Success<JobLease, string>(new JobLease.Leased(leasedUtc, WorkerId.From(workerId)));
    }
}