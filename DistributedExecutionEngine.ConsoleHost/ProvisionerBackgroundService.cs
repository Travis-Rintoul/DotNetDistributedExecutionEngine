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
        throw new NotImplementedException();
        
        // Console.WriteLine("[Provisioner] Provisioner started");
        //
        // while (!token.IsCancellationRequested)
        // {
        //     try
        //     {
        //         using var scope = scopeFactory.CreateScope();
        //
        //         var provisionerService = scope.ServiceProvider.GetRequiredService<IProvisionerService>();
        //         
        //         await provisionerService.StartScaling();
        //     }
        //     catch (Exception ex)
        //     {
        //         logger.LogError(ex, "[Provisioner] Provisioner scaling failed");
        //     }
        //
        //     await Task.Delay(TimeSpan.FromSeconds(1), token);
        // }
    }
}