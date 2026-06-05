using System.Diagnostics;
using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerStatuses;

public interface IWorkerStatusMapper : IStatusMapper<IWorkerStatusRecord, WorkerStatusCode, WorkerStatus> { }

internal sealed class WorkerStatusMapper : IWorkerStatusMapper
{
    public WorkerStatusCode ToCode(WorkerStatus status)
    {
        return status switch
        {
            WorkerStatus.Pending => WorkerStatusCode.Pending,
            WorkerStatus.Starting => WorkerStatusCode.Starting,
            WorkerStatus.Running => WorkerStatusCode.Running,
            WorkerStatus.Failed => WorkerStatusCode.Failed,
            WorkerStatus.Completed => WorkerStatusCode.Completed,
            WorkerStatus.Canceled => WorkerStatusCode.Canceled,
            _ => throw new UnreachableException()
        };
    }
    
    public Result<WorkerStatus, string> Map(IWorkerStatusRecord persistence)
    {
        return persistence.StatusCode switch
        {
            WorkerStatusCode.Pending => Result<WorkerStatus, string>.Success(new WorkerStatus.Pending()),
            WorkerStatusCode.Starting => Result<WorkerStatus, string>.Success(new WorkerStatus.Starting()),
            WorkerStatusCode.Running => Result<WorkerStatus, string>.Success(new WorkerStatus.Running()),
            WorkerStatusCode.Failed => FailedStatus(persistence),
            WorkerStatusCode.Completed => CompletedStatus(persistence),
            WorkerStatusCode.Canceled => CanceledStatus(persistence),
            _ => throw new UnreachableException()
        };
    }
    
    public void ApplyToRecord(WorkerStatus status, IWorkerStatusRecord record)
    {
        ResetStatusFields(record);
        
        switch (status)
        {
            case WorkerStatus.Failed failedStatus:
                record.FailedUtc = failedStatus.FailedUtc;
                record.FailureReason = failedStatus.FailReason;
                break;
            case WorkerStatus.Completed completedStatus:
                record.CompletedUtc = completedStatus.CompletedUtc;
                break;
            case WorkerStatus.Canceled canceledStatus:
                record.CanceledUtc = canceledStatus.CanceledUtc;
                record.CancellationReason = canceledStatus.CancelReason;
                break;
        }
    }

    private static void ResetStatusFields(IWorkerStatusRecord record)
    {
        record.StartedUtc = null;
        record.StoppedUtc = null;
        record.FailureReason = null;
        record.CompletedUtc = null;
        record.FailedUtc = null;
        record.FailureReason = null;
        record.CanceledUtc = null;
        record.CancellationReason = null;
    }
    
    private static Result<WorkerStatus, string> FailedStatus(IWorkerStatusRecord persistence)
    {
        if (persistence.FailedUtc is not { } failedUtc)
        {
            return Result<WorkerStatus, string>.Failure("Running Worker must have ClaimedUtc.");
        }
        
        if (persistence.FailureReason is not { } failedReason)
        {
            return Result<WorkerStatus, string>.Failure("Running Worker must have ClaimedUtc.");
        }

        return Result<WorkerStatus, string>.Success(new WorkerStatus.Failed(failedUtc, failedReason));
    }
    
    private static Result<WorkerStatus, string> CompletedStatus(IWorkerStatusRecord persistence)
    {
        if (persistence.CompletedUtc is not { } completedUtc)
        {
            return Result<WorkerStatus, string>.Failure("Running Worker must have ClaimedUtc.");
        }

        return Result<WorkerStatus, string>.Success(new WorkerStatus.Completed(completedUtc));
    }

    private static Result<WorkerStatus, string> CanceledStatus(IWorkerStatusRecord persistence)
    {
        if (persistence.CanceledUtc is not { } canceledUtc)
        {
            return Result<WorkerStatus, string>.Failure("Running Worker must have CanceledUtc.");
        }
        
        return Result<WorkerStatus, string>.Success(new WorkerStatus.Canceled(canceledUtc, persistence.CancellationReason ?? string.Empty));
    }
}