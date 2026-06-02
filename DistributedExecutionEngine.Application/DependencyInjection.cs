using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Jobs.Scheduling;
using DistributedExecutionEngine.Application.Features.Workers.Supervision;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedExecutionEngine.Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddApplicationServices()
        {
            services.RegisterUtilityServices();
            services.RegisterCommandServices();
            services.RegisterMessagingServices();
        }

        private void RegisterUtilityServices()
        {
            services.AddScoped<IClock, SystemClock>();
        }

        private void RegisterMessagingServices()
        {
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        }

        private void RegisterCommandServices()
        {
            services.Scan(scan => scan
                .FromAssemblies(typeof(DependencyInjection).Assembly)
                .AddClasses(classes => classes
                    .AssignableTo(typeof(ICommandHandler<,>))
                    .Where(type => type is { IsAbstract: false, IsInterface: false }))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
                .AddClasses(classes => classes
                    .AssignableTo(typeof(IQueryHandler<,>))
                    .Where(type => type is { IsAbstract: false, IsInterface: false }))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }
    }
}