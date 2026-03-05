namespace DistributedExecutionEngine.Library.Application.Worker;

[Flags]
public enum WorkerCapability
{
    None = 0,
    Cpu = 1,
    Gpu = 2
}