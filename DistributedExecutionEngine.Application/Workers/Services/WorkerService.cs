using System.Threading.Tasks;
using DistributedExecutionEngine.Domain.Entities;
using DistributedExecutionEngine.Domain.Repositories;

namespace DistributedExecutionEngine.Application.Workers.Services;

public sealed class WorkerService(IWorkerRepository workerRepository) : IWorkerService
{
    public async Task<int> RegisterWorker()
        => await workerRepository.RegisterWorkerAsync(new Worker());

    public async Task MarkRunning(Worker worker)
    {
        worker.MarkRunning();
        await workerRepository.SaveAsync(worker);
    }

    public async Task Heartbeat(Worker worker)
    {
        worker.UpdateHeartbeat();
        await workerRepository.SaveAsync(worker);
    }
}