using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Abstractions.Messaging;
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
            services.AddScoped<IQueryDispatcher, QueryDispatcher>();
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