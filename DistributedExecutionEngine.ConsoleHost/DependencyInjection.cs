using Microsoft.Extensions.DependencyInjection;

namespace DistributedExecutionEngine.ConsoleHost;

public static class DependencyInjection
{
    public static IServiceCollection AddProvisionerHost(this IServiceCollection services)
    {
        services.AddHostedService<ProvisionerBackgroundService>();
        return services;
    }
    
    public static IServiceCollection AddLauncherHost(this IServiceCollection services)
    {
        services.AddHostedService<WorkerLauncherBackgroundService>();
        return services;
    }
}