namespace DistributedExecutionEngine.Application.Workers.Services;

public interface IWorkerService
{
    public Task<int> RegisterWorker();
}