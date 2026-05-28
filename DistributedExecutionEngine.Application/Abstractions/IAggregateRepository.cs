using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Abstractions.Persistence;

public interface IAggregateRepository<TAggregate, in TKey>
{
    public Task<Option<TAggregate>> FindByKeyAsync(TKey key, CancellationToken ct = default);
    public Task AddAsync(TAggregate aggregate, CancellationToken ct = default);
    public Task UpdateAsync(TAggregate aggregate, CancellationToken ct = default);
}