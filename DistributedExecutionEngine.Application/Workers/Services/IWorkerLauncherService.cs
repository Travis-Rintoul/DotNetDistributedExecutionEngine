namespace DistributedExecutionEngine.Application.Workers.Services;

public interface IWorkerLauncherService
{
    public Task StartWorkerAsync(CancellationToken token);
    public Task StopWorkerAsync(int processId, CancellationToken token);
}