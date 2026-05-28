namespace DistributedExecutionEngine.Application.Abstractions.Messaging;

public interface ICommandDispatcher
{
    public Task<TResult> SendAsync<TCommand, TResult>(TCommand command);
}