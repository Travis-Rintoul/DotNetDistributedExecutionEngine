using Microsoft.Extensions.DependencyInjection;

namespace DistributedExecutionEngine.ConsoleHost;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddWorkerProvisionerHost()
            => services.AddHostedService<WorkerProvisionerBackgroundService>();

        public void AddSupervisorHost()
            => services.AddHostedService<WorkerSupervisorBackgroundService>();
    }
}