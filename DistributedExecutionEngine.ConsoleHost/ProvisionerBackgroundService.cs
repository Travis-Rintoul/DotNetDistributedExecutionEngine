using DistributedExecutionEngine.Application.Jobs.Services;
using DistributedExecutionEngine.Application.Provisioner;
using DistributedExecutionEngine.Application.Workers;
using DistributedExecutionEngine.Application.Workers.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DistributedExecutionEngine.ConsoleHost;

public sealed class ProvisionerBackgroundService(
    IWorkerService workerService,
    IJobService jobService,
    IJobExecutorService executor,
    IProvisionerService provisionerService,
    ILogger<ProvisionerBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        logger.LogInformation("Provisioner started");

        while (!token.IsCancellationRequested)
        {
            try
            {
                await provisionerService.StartScaling(token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Provisioner scaling failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(10), token);
        }
    }
}