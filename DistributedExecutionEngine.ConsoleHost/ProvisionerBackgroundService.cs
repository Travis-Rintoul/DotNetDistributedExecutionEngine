using DistributedExecutionEngine.Application.Jobs.Services;
using DistributedExecutionEngine.Application.Provisioner;
using DistributedExecutionEngine.Application.Workers;
using DistributedExecutionEngine.Application.Workers.Services;
using DistributedExecutionEngine.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DistributedExecutionEngine.ConsoleHost;

public sealed class ProvisionerBackgroundService(
    ILogger<ProvisionerBackgroundService> logger,
    IServiceScopeFactory scopeFactory
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        Console.WriteLine("[Provisioner] Provisioner started");

        while (!token.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();

                var provisionerService = scope.ServiceProvider.GetRequiredService<IProvisionerService>();
                
                await provisionerService.StartScaling(token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Provisioner] Provisioner scaling failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), token);
        }
    }
}