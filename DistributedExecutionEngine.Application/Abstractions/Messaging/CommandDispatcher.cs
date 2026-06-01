using Microsoft.Extensions.DependencyInjection;

namespace DistributedExecutionEngine.Application.Abstractions.Messaging;

public class CommandDispatcher(IServiceProvider serviceProvider) : ICommandDispatcher
{
    public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(
            command.GetType(),
            typeof(TResult));

        var handler = serviceProvider.GetRequiredService(handlerType);

        return ((dynamic)handler).HandleAsync((dynamic)command, cancellationToken);
    }
}