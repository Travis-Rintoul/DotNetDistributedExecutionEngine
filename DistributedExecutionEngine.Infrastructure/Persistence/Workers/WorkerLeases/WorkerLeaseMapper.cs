using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Domain.Aggregates.Supervisor;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerLeases;

public interface IWorkerLeaseMapper : IToDomainMapper<IWorkerLeaseRecord, WorkerLease>, IApplyToRecordMapper<WorkerLease, IWorkerLeaseRecord>;

public class WorkerLeaseMapper : IWorkerLeaseMapper
{
    public Result<WorkerLease, string> ToDomain(IWorkerLeaseRecord persistence)
    {
        return persistence.LeaseStatusCode switch
        {
            WorkerLeaseStatusCode.Available => ToAvailable(persistence),
            WorkerLeaseStatusCode.Leased => ToLeased(persistence),
            _ => Result<WorkerLease, string>.Failure(
                $"Unknown worker lease status code: {persistence.LeaseStatusCode}.")
        };
    }

    public void ApplyToRecord(WorkerLease domain, IWorkerLeaseRecord record)
    {
        ResetFields(record);

        if (domain is WorkerLease.Claimed claimed)
        {
            record.SupervisionLeasedBy = claimed.SupervisorId.Value;
            record.SupervisionLeasedUtc = claimed.ClaimedUtc;
            record.SupervisionLeaseExpiresUtc = claimed.ClaimedUtc;
        }
    }
    

    private void ResetFields(IWorkerLeaseRecord record)
    {
        record.SupervisionLeasedBy = null;
        record.SupervisionLeasedUtc = null;
        record.SupervisionLeaseExpiresUtc = null;
    }

    private static Result<WorkerLease, string> ToAvailable(IWorkerLeaseRecord persistence)
    {
        if (persistence.SupervisionLeasedBy is not null)
        {
            return Result<WorkerLease, string>.Failure(
                "Unclaimed worker lease must not have SupervisorId.");
        }

        if (persistence.SupervisionLeasedUtc is not null)
        {
            return Result<WorkerLease, string>.Failure(
                "Unclaimed worker lease must not have ClaimedUtc.");
        }

        return Result<WorkerLease, string>.Success(new WorkerLease.Unclaimed());
    }

    private static Result<WorkerLease, string> ToLeased(IWorkerLeaseRecord persistence)
    {
        if (persistence.SupervisionLeasedBy is not { } supervisorId)
        {
            return Result<WorkerLease, string>.Failure(
                "Claimed worker lease must have SupervisorId.");
        }

        if (persistence.SupervisionLeasedUtc is not { } claimedUtc)
        {
            return Result<WorkerLease, string>.Failure(
                "Claimed worker lease must have ClaimedUtc.");
        }
        
        if (persistence.SupervisionLeaseExpiresUtc is not { } expiresUtc)
        {
            return Result<WorkerLease, string>.Failure(
                "Claimed worker lease must have ClaimedUtc.");
        }

        return Result<WorkerLease, string>.Success(
            new WorkerLease.Claimed(SupervisorId.From(supervisorId), claimedUtc, expiresUtc));
    }
}