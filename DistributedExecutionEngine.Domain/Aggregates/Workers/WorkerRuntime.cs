using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Domain.Aggregates.Workers;

public abstract record WorkerRuntime
{
    public sealed record Pending() : WorkerRuntime;

    public sealed record Starting(
        ProcessId ProcessId,
        string Hostname,
        string MachineName,
        Option<DateTimeOffset> ProcessStartedUtc,
        Option<DateTimeOffset> LastHeartbeatUtc,
        int StartupAttemptCount,
        int MaxStartupAttemptCount) : WorkerRuntime;

    public sealed record Running(
        ProcessId ProcessId,
        string Hostname,
        string MachineName,
        Option<DateTimeOffset> ProcessStartedUtc,
        Option<DateTimeOffset> RunningSinceUtc,
        Option<DateTimeOffset> LastHeartbeatUtc,
        int StartupAttemptCount,
        int MaxStartupAttemptCount) : WorkerRuntime;

    public sealed record Failed(int StartupAttemptCount, int MaxStartupAttemptCount, DateTimeOffset? LastHeartbeatUtc) : WorkerRuntime;

    public sealed record Canceled(
        int StartupAttemptCount, 
        int MaxStartupAttemptCount) : WorkerRuntime;

    public sealed record Lost(
        ProcessId ProcessId,
        string Hostname,
        string MachineName,
        Option<DateTimeOffset> ProcessStartedUtc, 
        Option<DateTimeOffset> RunningSinceUtc, 
        Option<DateTimeOffset> LastHeartbeatUtc,
        int StartupAttemptCount, 
        int MaxStartupAttemptCount) : WorkerRuntime;
}