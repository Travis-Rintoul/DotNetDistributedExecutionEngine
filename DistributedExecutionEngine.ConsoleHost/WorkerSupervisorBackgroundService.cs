using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Workers.Lifecycle;
using DistributedExecutionEngine.Application.Features.Workers.Process;
using DistributedExecutionEngine.Application.Features.Workers.Supervision;
using DistributedExecutionEngine.Domain.Aggregates.Supervisor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DistributedExecutionEngine.ConsoleHost;

public class WorkerSupervisorBackgroundService(
    ILogger<WorkerSupervisorBackgroundService> logger,
    IWorkerProcessLauncher workerLauncher,
    IServiceScopeFactory scopeFactory
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        logger.LogInformation("[WorkerSupervisor] Starting...");

        while (!token.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1), token);

            var supervisorId = new SupervisorId(Environment.ProcessId);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var dispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

                var claimPendingResult = await dispatcher.SendAsync(new ClaimPendingWorkersForSupervisorCommand(supervisorId), token);
                if (claimPendingResult.IsFailure)
                    logger.LogError("[WorkerSupervisor] ClaimPendingWorkersForSupervisorCommand failed");

                foreach (var pendingWorkerId in claimPendingResult.Value)
                {
                    logger.LogInformation("[WorkerSupervisor] claimed worker {PendingWorkerId}", pendingWorkerId);
                    
                    var launchWorkerResult = await dispatcher.SendAsync(new LaunchWorkerCommand(pendingWorkerId, supervisorId), token);
                    if (launchWorkerResult.IsSuccess)
                        logger.LogInformation("[WorkerSupervisor] launched worker {PendingWorkerId} ProcessId: {ProcessId}", pendingWorkerId, launchWorkerResult.Value);
                    else
                        logger.LogError("[WorkerSupervisor] Launching worker failed");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Provisioner] Provisioner scaling failed");
            }
        }
    }
}