using System.Threading.Tasks;

namespace DistributedExecutionEngine.Application.Provisioner;

public interface IProvisionerService
{
    public Task StartScaling();
}