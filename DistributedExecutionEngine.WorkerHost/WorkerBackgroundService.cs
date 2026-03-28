using DistributedExecutionEngine.Application.Jobs.Services;
using DistributedExecutionEngine.Application.Workers;
using DistributedExecutionEngine.Application.Workers.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DistributedExecutionEngine.WorkerHost;

public sealed class WorkerBackgroundService(
    IWorkerService workerService,
    IJobService jobService,
    IJobExecutorService executor,
    ILogger<WorkerBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        logger.LogWarning("[worker)] Starting Worker...)]");
        
        var workerId = await workerService.RegisterWorker();

        while (!token.IsCancellationRequested)
        {
            //var job = await jobService.LeaseJob(workerId);

            await Task.Delay(500, token);
            
            logger.LogInformation($"[worker ({workerId})] worker waiting)]");
        }
    }
}