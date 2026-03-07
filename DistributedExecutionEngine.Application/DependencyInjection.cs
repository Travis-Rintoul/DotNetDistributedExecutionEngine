using DistributedExecutionEngine.Application.Jobs.Services;
using DistributedExecutionEngine.Application.Provisioner;
using DistributedExecutionEngine.Application.Workers.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedExecutionEngine.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IJobService, JobService>();
        services.AddScoped<IJobExecutorService, JobExecutorService>();
        services.AddScoped<IWorkerService, WorkerService>();
        services.AddScoped<IProvisionerService, ProvisionerService>();
        services.AddSingleton<IWorkerLauncherService, WorkerLauncherService>();

        return services;
    }
}