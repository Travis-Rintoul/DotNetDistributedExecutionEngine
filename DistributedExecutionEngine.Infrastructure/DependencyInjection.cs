using DistributedExecutionEngine.Application.Abstractions.Persistence;
using DistributedExecutionEngine.Application.Features.Workers.Lifecycle;
using DistributedExecutionEngine.Application.Features.Workers.Persistence;
using DistributedExecutionEngine.Application.Features.Workers.Supervision;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Infrastructure.Persistence;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedExecutionEngine.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IWorkerPoolStore, WorkerPoolStore>();
        services.AddScoped<IWorkerProcessLauncher, ProcessWorkerLauncher>();
        services.AddScoped<IAggregateRepository<Job, JobId>, JobRepository>();
        services.AddScoped<IAggregateRepository<Worker, WorkerId>, WorkerRepository>();
        services.AddDbContext<DistributedExecutionDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("Default"));
        });
    }
}