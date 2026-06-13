using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerLeases;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerStatuses;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers;

public interface IWorkerMapper : IAggregateMapper<WorkerRecord, Worker>;

internal sealed class WorkerMapper(IWorkerStatusMapper workerStatusMapper, IWorkerLeaseMapper workerLeaseMapper) : IWorkerMapper
{
    public Result<Worker, string> ToDomain(WorkerRecord persistence)
    {
        Result<WorkerStatus, string> status = workerStatusMapper.Map(persistence);
        if (status.IsFailure)
        {
            return Result<Worker, string>.Failure(status.Error);
        }
        
        Result<WorkerLease, string> lease = workerLeaseMapper.ToDomain(persistence);
        if (status.IsFailure)
        {
            return Result<Worker, string>.Failure(lease.Error);
        }
        
        return Result<Worker, string>.Success(
            Worker.Rehydrate(
                workerId: persistence.WorkerId,
                hostname: persistence.Hostname,
                status: status.Value,
                lease: lease.Value,
                maxConcurrency: persistence.MaxConcurrency,
                createdUtc: persistence.CreatedUtc
            )
        );
    }

    public WorkerRecord ToPersistence(Worker domain)
    {
        var record = new WorkerRecord();
        ApplyToRecord(domain, record);
        return record;
    }

    public void ApplyToRecord(Worker domain, WorkerRecord record)
    {
        record.WorkerId = domain.WorkerId.Value;
        record.Hostname = domain.Hostname;
        record.CreatedUtc = domain.CreatedUtc;
        record.StatusCode = workerStatusMapper.ToCode(domain.Status);
        record.MaxConcurrency = domain.MaxConcurrency;
        workerStatusMapper.ApplyToRecord(domain.Status, record);
        workerLeaseMapper.ApplyToRecord(domain.Lease, record);
    }
}