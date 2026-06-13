using Microsoft.Extensions.DependencyInjection;

namespace DistributedExecutionEngine.Application.Abstractions.Messaging;

public class QueryDispatcher(IServiceProvider serviceProvider) : IQueryDispatcher
{
    public Task<TResult> SendAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(
            query.GetType(),
            typeof(TResult));

        var handler = serviceProvider.GetRequiredService(handlerType);

        return ((dynamic)handler).HandleAsync((dynamic)query, cancellationToken);
    }
}