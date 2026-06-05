using DistributedExecutionEngine.Domain.Aggregates.Supervisor;

namespace DistributedExecutionEngine.Domain.Aggregates.Workers;

public abstract record WorkerStatus
{
    public sealed record Pending() : WorkerStatus;
    public sealed record Starting: WorkerStatus;
    public sealed record Running : WorkerStatus;
    public sealed record Failed(DateTimeOffset FailedUtc, string FailReason) : WorkerStatus;
    public sealed record Completed(DateTimeOffset CompletedUtc) : WorkerStatus;
    public sealed record Canceled(DateTimeOffset CanceledUtc, string CancelReason) : WorkerStatus;
}