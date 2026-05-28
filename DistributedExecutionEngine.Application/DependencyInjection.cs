using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Jobs.Scheduling;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedExecutionEngine.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<ScheduleJobCommand, Result<JobId, ScheduleJobError>>, ScheduleJobCommandHandler>();
        
        return services;
    }
}