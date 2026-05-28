using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Abstractions.Persistence;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers;

public sealed class WorkerRepository(DistributedExecutionDbContext context, IAggregateMapper<WorkerRecord, Worker> workerMapper) : IAggregateRepository<Worker, WorkerId>
{
    private async Task<WorkerRecord?> FindRecordByKey(WorkerId key, CancellationToken ct = default) => 
        await context.Workers
            .SingleOrDefaultAsync(x => x.WorkerId == key.Value, cancellationToken: ct);

    public async Task<Option<Worker>> FindByKeyAsync(WorkerId key, CancellationToken ct = default)
    {
        var record = await FindRecordByKey(key, ct);
        if (record == null)
            return Option<Worker>.None;
        
        return WorkerMapper.ToDomain(record).Match(
            success: Option<Worker>.Some,
            failure: error => throw new InvalidOperationException(
                $"Failed to map JobRecord '{record.WorkerId}' to domain Job. Error: {error}"));
    }

    public async Task AddAsync(Worker aggregate, CancellationToken ct = default)
    {
        var record = WorkerMapper.ToPersistence(aggregate);
        await context.Workers.AddAsync(record, ct);
    }

    public async Task UpdateAsync(Worker aggregate, CancellationToken ct = default)
    {
        
        throw new NotImplementedException();
    }
}