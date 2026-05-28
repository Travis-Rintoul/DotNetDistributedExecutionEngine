namespace DistributedExecutionEngine.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}