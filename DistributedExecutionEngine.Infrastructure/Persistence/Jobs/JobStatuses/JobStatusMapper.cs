using System.Diagnostics;
using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobStatuses;

public interface IJobStatusMapper : IStatusMapper<IJobStatusRecord, JobStatusCode, JobStatus>;

internal sealed class JobStatusMapper : IJobStatusMapper
{
    public JobStatusCode ToCode(JobStatus status)
    {
        return status switch
        {
            JobStatus.Pending => JobStatusCode.Pending,
            JobStatus.Running => JobStatusCode.Running,
            JobStatus.Succeeded => JobStatusCode.Succeeded,
            JobStatus.Failed => JobStatusCode.Failed,
            _ => throw new UnreachableException()
        };
    }
    
    public Result<JobStatus, string> Map(IJobStatusRecord persistence)
    {
        return persistence.StatusCode switch
        {
            JobStatusCode.Pending => Result<JobStatus, string>.Success(new JobStatus.Pending()),
            JobStatusCode.Running => RunningStatus(persistence),
            JobStatusCode.Succeeded => SucceededStatus(persistence),
            JobStatusCode.Failed => FailedStatus(persistence),
            _ => throw new UnreachableException()
        };
    }

    public void ApplyToRecord(JobStatus status, IJobStatusRecord record)
    {
        ResetFields(record);
        
        switch (status)
        {
            case JobStatus.Pending:
                break;

            case JobStatus.Running running:
                record.StartedUtc = running.StartedUtc;
                break;
            
            case JobStatus.Succeeded succeeded:
                record.StartedUtc = succeeded.StartedUtc;
                record.CompletedUtc = succeeded.CompletedUtc;
                break;
            
            case JobStatus.Failed failed:
                record.FailureReason = failed.Reason;
                record.FailedUtc = failed.FailedUtc;
                break;

            default:
                throw new UnreachableException(
                    $"Unknown {nameof(JobLease)} type: {record.GetType().Name}");
        }
    }

    private static void ResetFields(IJobStatusRecord record)
    {
        record.FailureReason = null;
        record.StartedUtc = null;
        record.CompletedUtc = null;
        record.FailedUtc = null;
        record.FailedUtc = null;
    }

    private static Result<JobStatus, string> RunningStatus(IJobStatusRecord persistence)
    {
        if (persistence.StartedUtc is not { } startedUtc)
        {
            return Result<JobStatus, string>.Failure("Running job must have StartedUtc.");
        }

        return Result<JobStatus, string>.Success(new JobStatus.Running(startedUtc));
    }

    private Result<JobStatus, string> SucceededStatus(IJobStatusRecord persistence)
    {
        if (persistence.StartedUtc is not { } startedUtc)
        {
            return Result<JobStatus, string>.Failure("Succeeded job must have StartedUtc.");
        }

        if (persistence.CompletedUtc is not { } completedUtc)
        {
            return Result<JobStatus, string>.Failure("Succeeded job must have CompletedUtc.");
        }

        return Result<JobStatus, string>.Success(new JobStatus.Succeeded(startedUtc, completedUtc));
    }

    private Result<JobStatus, string> FailedStatus(IJobStatusRecord persistence)
    {
        if (persistence.FailedUtc is not { } failedUtc)
        {
            return Result<JobStatus, string>.Failure("Failed job must have FailedUtc.");
        }

        return Result<JobStatus, string>.Success(new JobStatus.Failed(failedUtc,
            persistence.FailureReason ?? string.Empty));
    }
}