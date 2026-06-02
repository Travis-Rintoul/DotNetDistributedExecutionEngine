using DistributedExecutionEngine.Application.Abstractions.Persistence;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Jobs;

public sealed class JobRepository(DistributedExecutionDbContext context) : IAggregateRepository<Job, JobId>
{
    private async Task<JobRecord?> FindRecordByKey(Guid jobId, CancellationToken ct) =>
        await context.Jobs
            .SingleOrDefaultAsync(job => job.JobId == jobId, cancellationToken: ct);
    
    public async Task<Option<Job>> FindByKeyAsync(JobId key, CancellationToken ct = default)
    {
        var record = await FindRecordByKey(key.Value, ct);
        if (record is null)
            return Option<Job>.None;

        return JobMapper.ToDomain(record).Match(
            success: Option<Job>.Some,
            failure: error => throw new InvalidOperationException(
                $"Failed to map JobRecord '{record.JobId}' to domain Job. Error: {error}"));
    }

    public async Task AddAsync(Job aggregate, CancellationToken ct = default)
    {
        var record = JobMapper.ToPersistence(aggregate);
        await context.Jobs.AddAsync(record, ct);
    }

    public async Task UpdateAsync(Job aggregate, CancellationToken ct = default)
    {
        var record = await FindRecordByKey(aggregate.JobId.Value, ct);
        if (record is null)
            throw new InvalidOperationException($"Job record '{aggregate.JobId.Value}' not found.");

        JobMapper.ApplyToRecord(aggregate, record);
    }
}