namespace DistributedExecutionEngine.Domain.Enums;

[Flags]
public enum WorkerCapability
{
    None = 0,
    Cpu = 1,
    Gpu = 2
}