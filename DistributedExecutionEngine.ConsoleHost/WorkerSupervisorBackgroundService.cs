using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Workers.Supervision;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DistributedExecutionEngine.ConsoleHost;

public class WorkerSupervisorBackgroundService(
    ILogger<WorkerSupervisorBackgroundService> logger,
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
                var superviseResult = await dispatcher.SendAsync(new SuperviseWorkersCommand(supervisorId), token);
                if (superviseResult.IsFailure)
                {
                    logger.LogError("[WorkerSupervisor] SuperviseWorkersCommand failed");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Provisioner] Provisioner scaling failed");
            }
        }
        
        throw new NotImplementedException();
    }
}