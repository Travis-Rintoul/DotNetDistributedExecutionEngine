using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DistributedExecutionEngine.WorkerHost;

public sealed class WorkerOptions
{
    public int WorkerId { get; set; }
}

public sealed class WorkerBackgroundService(IServiceScopeFactory scopeFactory, ILogger<WorkerBackgroundService> logger, IOptions<WorkerOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        throw new NotImplementedException();
        // var workerId = options.Value.WorkerId;
        //
        // logger.LogWarning($"Starting Worker ({workerId})...)]");
        //
        // while (!token.IsCancellationRequested)
        // {
        //     using var scope = scopeFactory.CreateScope();
        //     var jobService = scope.ServiceProvider.GetRequiredService<IJobService>();
        //     var jobExecutorService = scope.ServiceProvider.GetRequiredService<IJobExecutorService>();
        //     
        //     try
        //     {
        //
        //         
        //     }
        //     catch (Exception ex)
        //     {
        //         logger.LogError(ex, "Worker loop failure");
        //         await Task.Delay(TimeSpan.FromSeconds(5), token); // backoff
        //     }
        // }
    }
}