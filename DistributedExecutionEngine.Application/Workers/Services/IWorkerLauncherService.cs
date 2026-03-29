using System.Threading;
using System.Threading.Tasks;

namespace DistributedExecutionEngine.Application.Workers.Services;

public interface IWorkerLauncherService
{
    public Task StartWorkerAsync(int workerId, CancellationToken token);
    public Task StopWorkerAsync(int processId, CancellationToken token);
}