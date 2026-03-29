using System.Threading.Tasks;
using DistributedExecutionEngine.Domain.Entities;

namespace DistributedExecutionEngine.Application.Workers.Services;

public interface IWorkerService
{
    public Task MarkRunning(Worker worker);
    public Task Heartbeat(Worker worker);
}