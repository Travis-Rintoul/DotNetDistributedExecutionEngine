namespace DistributedExecutionEngine.Application.Abstractions.Messaging;

public interface ICommandDispatcher
{
    public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}