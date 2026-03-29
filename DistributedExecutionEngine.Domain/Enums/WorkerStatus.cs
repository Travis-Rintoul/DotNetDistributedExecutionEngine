namespace DistributedExecutionEngine.Library.Application.Worker;

public enum WorkerStatus
{
    Pending = 0,  // Created, not yet claimed
    Starting = 1, // Claimed by launcher, process booting
    Running = 2,  // Healthy and processing
    Failed = 3,   // Failed to start or crashed
    Stopped = 4   // Gracefully stopped (optional but useful)
}