using System;
using System.Threading;
using System.Threading.Tasks;
using DistributedExecutionEngine.Application.Workers.Services;
using DistributedExecutionEngine.Domain.Entities;
using DistributedExecutionEngine.Domain.Repositories;

namespace DistributedExecutionEngine.Application.Provisioner;

public sealed class ProvisionerService(
    IWorkerLauncherService workerLauncherService,
    IJobRepository  jobRepository,
    IWorkerRepository workerRepository
) : IProvisionerService
{
    public async Task StartScaling()
    {
        var pending = await jobRepository.CountPendingAsync();
        var workers = await workerRepository.CountAsync();

        if (pending - workers > workers)
        {
            Console.WriteLine($"[Provisioner] {pending} workers were provisioned");
            await workerRepository.RegisterWorkerAsync(Worker.Create("worker"));
        }
    }
}