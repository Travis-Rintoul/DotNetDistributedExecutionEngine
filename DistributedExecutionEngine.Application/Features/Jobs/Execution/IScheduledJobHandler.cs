namespace DistributedExecutionEngine.Application.Features.Jobs.Execution;

public interface IScheduledJobHandler<in TQuery, TResult>
    where TQuery : IScheduledJobPayload<TResult>
{
    public Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}