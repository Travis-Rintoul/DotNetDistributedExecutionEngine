using Microsoft.Extensions.DependencyInjection;

namespace DistributedExecutionEngine.ConsoleHost;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkerProvisionerHost(this IServiceCollection services)
    {
        services.AddHostedService<WorkerProvisionerBackgroundService>();
        return services;
    }
    
    public static IServiceCollection AddSupervisorHost(this IServiceCollection services)
    {
        services.AddHostedService<WorkerSupervisorBackgroundService>();
        return services;
    }
}