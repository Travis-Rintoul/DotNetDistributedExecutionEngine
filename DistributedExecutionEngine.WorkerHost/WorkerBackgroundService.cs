using DistributedExecutionEngine.Application.Jobs.Services;
using DistributedExecutionEngine.Application.Workers;
using DistributedExecutionEngine.Application.Workers.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DistributedExecutionEngine.WorkerHost;

public sealed class WorkerBackgroundService(
    IWorkerService workerService,
    IJobService jobService,
    IJobExecutorService executor,
    ILogger<WorkerBackgroundService> logger,
    IConfiguration config
) : BackgroundService
{
    private readonly int _workerId = int.Parse(config["worker-id"]!);
    
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        logger.LogWarning($"Starting Worker ({_workerId})...)]");
        
        while (!token.IsCancellationRequested)
        {
            var job = await jobService.LeaseJob(_workerId);

            if (job != null)
            {
                logger.LogInformation($"Worker ({_workerId}) found job: {job.Id}");

                var result = await executor.ExecuteJob(job);
                
                logger.LogInformation($"JOB RESULT: {result.Message}");
            }
            
            await Task.Delay(TimeSpan.FromSeconds(1), token);   
        }
    }
}