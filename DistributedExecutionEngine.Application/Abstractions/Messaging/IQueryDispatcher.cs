namespace DistributedExecutionEngine.Application.Abstractions.Messaging;

public interface IQueryDispatcher
{
    public Task<TResult> SendAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default);
}