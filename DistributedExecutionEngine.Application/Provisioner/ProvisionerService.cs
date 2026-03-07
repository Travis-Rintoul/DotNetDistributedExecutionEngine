using DistributedExecutionEngine.Application.Workers.Services;
using DistributedExecutionEngine.Domain.Repositories;

namespace DistributedExecutionEngine.Application.Provisioner;

public sealed class ProvisionerService(
    IWorkerLauncherService workerLauncherService,
    IJobRepository  jobRepository,
    IWorkerRepository workerRepository
) : IProvisionerService
{
    public async Task StartScaling(CancellationToken cancellationToken)
    {
        var pending = await jobRepository.PendingJobsCountAsync();
        var workers = await workerRepository.Count();

        if (pending > workers)
            await workerLauncherService.StartWorkerAsync(cancellationToken);
    }
}