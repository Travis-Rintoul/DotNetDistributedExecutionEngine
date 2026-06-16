using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Abstractions.Persistence;
using DistributedExecutionEngine.Application.Features.Jobs.Leasing;
using DistributedExecutionEngine.Application.Features.Jobs.Persistence;
using DistributedExecutionEngine.Application.Features.JobTypes.Persistence;
using DistributedExecutionEngine.Application.Features.Workers.Persistence;
using DistributedExecutionEngine.Application.Features.Workers.Process;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Infrastructure.Persistence;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobLeasing;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobStatuses;
using DistributedExecutionEngine.Infrastructure.Persistence.JobTypes;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers.Runtime;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerLeases;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerStatuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IWorkerQueries = DistributedExecutionEngine.Application.Abstractions.Persistence.IWorkerQueries;
using IWorkerRepository = DistributedExecutionEngine.Application.Abstractions.Persistence.IWorkerRepository;

namespace DistributedExecutionEngine.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IWorkerMapper, WorkerMapper>();
        services.AddScoped<IWorkerStatusMapper, WorkerStatusMapper>();
        services.AddScoped<IWorkerLeaseMapper, WorkerLeaseMapper>();
        services.AddScoped<IWorkerRuntimeMapper, WorkerRuntimeMapper>();
        
        services.AddScoped<IJobMapper, JobMapper>();
        services.AddScoped<IJobStatusMapper, JobStatusMapper>();
        services.AddScoped<IJobLeaseMapper, JobLeaseMapper>();
        
        services.AddScoped<IWorkerPoolStore, WorkerPoolStore>();
        services.AddScoped<IWorkerProcessLauncher, ProcessWorkerLauncher>();
        
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IWorkerRepository, WorkerRepository>();
       
        services.AddScoped<IJobQueries, JobQueries>();
        services.AddScoped<IWorkerQueries, WorkerQueries>();
        services.AddScoped<IJobTypesQueries, JobTypeQueries>();
        services.AddScoped<IJobLeaseStore, JobLeaseStore>();
        
        services.AddScoped<IAggregateRepository<Job, JobId>, JobRepository>();
        services.AddScoped<IAggregateRepository<Worker, WorkerId>, WorkerRepository>();
        
        services.AddDbContext<DistributedExecutionDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("Default"));
        });
        
        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<DistributedExecutionDbContext>());
    }
}