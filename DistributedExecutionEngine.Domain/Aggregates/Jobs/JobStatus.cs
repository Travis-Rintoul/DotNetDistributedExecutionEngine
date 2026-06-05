using System.Diagnostics;

namespace DistributedExecutionEngine.Domain.Aggregates.Jobs;

public abstract record JobStatus
{
    public sealed record Pending() : JobStatus;

    public sealed record Running(DateTimeOffset StartedUtc) : JobStatus;

    public sealed record Succeeded(DateTimeOffset StartedUtc, DateTimeOffset CompletedUtc) : JobStatus;

    public sealed record Failed(DateTimeOffset FailedUtc, string Reason = "") : JobStatus;
}