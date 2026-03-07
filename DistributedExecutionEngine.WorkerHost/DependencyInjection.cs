using DistributedExecutionEngine.Domain.Repositories;
using DistributedExecutionEngine.Infrastructure.Persistence;
using DistributedExecutionEngine.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedExecutionEngine.WorkerHost;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkerHost(this IServiceCollection services)
    {
        services.AddHostedService<WorkerBackgroundService>();
        return services;
    }
}