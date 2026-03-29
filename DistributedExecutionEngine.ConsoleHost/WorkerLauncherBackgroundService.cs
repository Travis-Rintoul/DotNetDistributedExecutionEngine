using DistributedExecutionEngine.Application.Provisioner;
using DistributedExecutionEngine.Application.Workers.Services;
using DistributedExecutionEngine.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DistributedExecutionEngine.ConsoleHost;

public class WorkerLauncherBackgroundService(IServiceScopeFactory scopeFactory, ILogger<ProvisionerBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        Console.WriteLine("[WorkerLauncher] Starting...");
        while (!token.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();

                var repo = scope.ServiceProvider.GetRequiredService<IWorkerRepository>();
                var workerService = scope.ServiceProvider.GetRequiredService<IWorkerService>();
                var launcherService = scope.ServiceProvider.GetRequiredService<IWorkerLauncherService>();

                var worker = await repo.ClaimPendingWorkerAsync();
                if (worker != null)
                {
                    Console.WriteLine($"[WorkerLauncher] claimed worker {worker.Id}");
                    await launcherService.StartWorkerAsync(worker.Id,  token);
                    await workerService.MarkRunning(worker);
                }
                
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Provisioner scaling failed");
                Console.WriteLine($"[WorkerLauncher] claim failed");
            }

            await Task.Delay(1000, token);
        }
    }
}