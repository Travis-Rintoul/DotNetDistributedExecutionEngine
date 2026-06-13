using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobLeasing;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobStatuses;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerLeases;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Jobs;

public interface IJobMapper : IAggregateMapper<JobRecord, Job> { }

internal sealed class JobMapper(
    IJobStatusMapper statusMapper,
    IJobLeaseMapper leaseMapper
) : IJobMapper
{
    public Result<Job, string> ToDomain(JobRecord persistence)
    {
        Result<JobStatus, string> status = statusMapper.Map(persistence);
        if (status.IsFailure)
        {
            return Result<Job, string>.Failure(status.Error);
        }
        
        Result<JobLease, string> lease = leaseMapper.ToDomain(persistence);
        if (lease.IsFailure)
        {
            return Result<Job, string>.Failure(lease.Error);
        }

        return Result<Job, string>.Success(
            Job.Rehydrate(
                jobId: persistence.JobId,
                jobType: persistence.JobTypeCode,
                payloadJson: persistence.PayloadJson,
                status: status.Value,
                lease: lease.Value,
                createdUtc: persistence.CreatedUtc,
                attemptsCount: persistence.AttemptsCount,
                maxAttemptsCount: persistence.MaxAttemptsCount,
                assignedWorkerId: Option
                    .FromNullable(persistence.AssignedWorkerId)
                    .Map(WorkerId.From)
            )
        );
    }

    public JobRecord ToPersistence(Job domain)
    {
        var record = new JobRecord();
        ApplyToRecord(domain, record);
        return record;
    }

    public void ApplyToRecord(Job domain, JobRecord record)
    {
        record.JobId = domain.JobId.Value;
        record.CreatedUtc = domain.CreatedUtc;
        record.StatusCode = statusMapper.ToCode(domain.Status);
        record.PayloadJson = domain.PayloadJson;
        record.JobTypeCode = domain.JobType;
        record.AssignedWorkerId = domain.AssignedWorkerId.IsSome ? domain.AssignedWorkerId.Value.Value : null;
        record.AttemptsCount = domain.AttemptsCount;
        record.MaxAttemptsCount = domain.MaxAttemptsCount;
    }
}