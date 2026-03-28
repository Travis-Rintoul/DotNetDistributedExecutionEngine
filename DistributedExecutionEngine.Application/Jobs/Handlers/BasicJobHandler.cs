
namespace DistributedExecutionEngine.Application.Jobs.Handlers;

public sealed class BasicJob
{
    public string Foo { get; set; } = string.Empty;
}

public sealed class BasicJobHandler : IJobHandler<BasicJob>
{
    public Task<JobResult> ExecuteAsync(BasicJob job, CancellationToken cancellationToken)
    {
        return Task.FromResult(new JobResult { Message = "Hello!" });
    }
}