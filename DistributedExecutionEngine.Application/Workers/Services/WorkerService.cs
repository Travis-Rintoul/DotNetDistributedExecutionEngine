using DistributedExecutionEngine.Domain.Entities;
using DistributedExecutionEngine.Domain.Repositories;

namespace DistributedExecutionEngine.Application.Workers.Services;

public sealed class WorkerService(IWorkerRepository workerRepository) : IWorkerService
{
    public async Task<int> RegisterWorker()
        => await workerRepository.RegisterWorkerAsync(new Worker());
}