namespace DistributedExecutionEngine.Application.Provisioner;

public interface IProvisionerService
{
    public Task StartScaling(CancellationToken cancellationToken);
}