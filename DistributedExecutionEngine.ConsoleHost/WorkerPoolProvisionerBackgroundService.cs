using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Workers.Reconciliation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DistributedExecutionEngine.ConsoleHost;

public class WorkerProvisionerBackgroundService(
    ILogger<WorkerProvisionerBackgroundService> logger,
    IServiceScopeFactory scopeFactory
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        logger.LogInformation("[WorkerProvisioner] Starting...");
        
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), token);
            
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

                var reconcileResult = await dispatcher.SendAsync(new ReconcileWorkerPoolCommand(), token);
                if (reconcileResult.IsFailure)
                {
                    logger.LogError("[WorkerProvisioner] {ReconcileResultError}", reconcileResult.Error);
                    continue;
                }
                
                var reconcileOption = reconcileResult.Value;
                if (reconcileOption.IsNone)
                {
                    continue;
                }
                
                logger.LogInformation("[WorkerProvisioner] created new worker ({WorkerId})", reconcileOption.Value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Provisioner] Provisioner scaling failed");
            }
        }
    }
}