using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers.Runtime;
using FluentAssertions;

namespace DotNetDistributedExecutionEngine.Infrastructure.Tests;

public sealed class WorkerRuntimeMapperTests
{
    private readonly IWorkerRuntimeMapper _mapper = new WorkerRuntimeMapper();

    private static readonly DateTimeOffset ProcessStartedUtc =
        new(2026, 6, 15, 1, 0, 0, TimeSpan.Zero);

    private static readonly DateTimeOffset RunningSinceUtc =
        new(2026, 6, 15, 1, 1, 0, TimeSpan.Zero);

    private static readonly DateTimeOffset LastHeartbeatUtc =
        new(2026, 6, 15, 1, 2, 0, TimeSpan.Zero);

    [Fact]
    public void ToDomain_WhenPendingRecordIsValid_ShouldMapToPending()
    {
        var record = new WorkerRecord
        {
            StatusCode = WorkerStatusCode.Pending
        };

        var result = _mapper.ToDomain(record);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<WorkerRuntime.Pending>();
    }

    [Fact]
    public void ToDomain_WhenStartingRecordIsValid_ShouldMapToStarting()
    {
        var record = ValidStartingRecord();

        var result = _mapper.ToDomain(record);

        result.IsSuccess.Should().BeTrue();

        var starting = result.Value.Should().BeOfType<WorkerRuntime.Starting>().Subject;

        starting.ProcessId.Value.Should().Be(123);
        starting.Hostname.Should().Be("worker-host");
        starting.MachineName.Should().Be("worker-machine");
        starting.ProcessStartedUtc.Value.Should().Be(ProcessStartedUtc);
        starting.StartupAttemptCount.Should().Be(1);
        starting.MaxStartupAttemptCount.Should().Be(3);
    }

    [Fact]
    public void ToDomain_WhenRunningRecordIsValid_ShouldMapToRunning()
    {
        var record = ValidRunningRecord();

        var result = _mapper.ToDomain(record);

        result.IsSuccess.Should().BeTrue();

        var running = result.Value.Should().BeOfType<WorkerRuntime.Running>().Subject;

        running.ProcessId.Value.Should().Be(123);
        running.Hostname.Should().Be("worker-host");
        running.MachineName.Should().Be("worker-machine");
        running.ProcessStartedUtc.Value.Should().Be(ProcessStartedUtc);
        running.RunningSinceUtc.Value.Should().Be(RunningSinceUtc);
        running.LastHeartbeatUtc.Value.Should().Be(LastHeartbeatUtc);
        running.StartupAttemptCount.Should().Be(1);
        running.MaxStartupAttemptCount.Should().Be(3);
    }

    [Fact]
    public void ToDomain_WhenFailedRecordIsValid_ShouldMapToFailed()
    {
        var record = ValidFailedRecord();

        var result = _mapper.ToDomain(record);

        result.IsSuccess.Should().BeTrue();

        var failed = result.Value.Should().BeOfType<WorkerRuntime.Failed>().Subject;

        failed.StartupAttemptCount.Should().Be(2);
        failed.MaxStartupAttemptCount.Should().Be(3);
    }

    [Fact]
    public void ToDomain_WhenCanceledRecordIsValid_ShouldMapToCanceled()
    {
        var record = ValidCanceledRecord();

        var result = _mapper.ToDomain(record);

        result.IsSuccess.Should().BeTrue();

        var canceled = result.Value.Should().BeOfType<WorkerRuntime.Canceled>().Subject;

        canceled.StartupAttemptCount.Should().Be(1);
        canceled.MaxStartupAttemptCount.Should().Be(3);
    }

    [Fact]
    public void ToDomain_WhenPendingRecordHasProcessFields_ShouldFail()
    {
        var record = new WorkerRecord
        {
            StatusCode = WorkerStatusCode.Pending,
            ProcessId = 123,
            Hostname = "worker-host",
            MachineName = "worker-machine",
            ProcessStartedUtc = ProcessStartedUtc,
            RunningSinceUtc = RunningSinceUtc,
            LastHeartbeatUtc = LastHeartbeatUtc
        };

        var result = _mapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Forbidden field");
    }

    [Fact]
    public void ToDomain_WhenPendingRecordHasStartupAttemptFields_ShouldFail()
    {
        var record = new WorkerRecord
        {
            StatusCode = WorkerStatusCode.Pending,
            StartupAttemptCount = 1,
            MaxStartupAttemptCount = 3
        };

        var result = _mapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Forbidden field");
    }

    [Fact]
    public void ToDomain_WhenStartingRecordIsMissingProcessId_ShouldFail()
    {
        var record = ValidStartingRecord();
        record.ProcessId = null;

        var result = _mapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("ProcessId");
    }

    [Fact]
    public void ToDomain_WhenStartingRecordIsMissingHostname_ShouldFail()
    {
        var record = ValidStartingRecord();
        record.Hostname = null;

        var result = _mapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Hostname");
    }

    [Fact]
    public void ToDomain_WhenStartingRecordIsMissingMachineName_ShouldFail()
    {
        var record = ValidStartingRecord();
        record.MachineName = null;

        var result = _mapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("MachineName");
    }

    [Fact]
    public void ToDomain_WhenStartingRecordIsMissingProcessStartedUtc_ShouldFail()
    {
        var record = ValidStartingRecord();
        record.ProcessStartedUtc = null;

        var result = _mapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("ProcessStartedUtc");
    }

    [Fact]
    public void ToDomain_WhenRunningRecordIsMissingRunningSinceUtc_ShouldFail()
    {
        var record = ValidRunningRecord();
        record.RunningSinceUtc = null;

        var result = _mapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("RunningSinceUtc");
    }

    [Fact]
    public void ToDomain_WhenStartingRecordHasRunningSinceUtc_ShouldFail()
    {
        var record = ValidStartingRecord();
        record.RunningSinceUtc = RunningSinceUtc;

        var result = _mapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("RunningSinceUtc");
    }

    [Fact]
    public void ToDomain_WhenFailedRecordHasProcessFields_ShouldFail()
    {
        var record = ValidFailedRecord();
        record.ProcessId = 123;
        record.Hostname = "worker-host";
        record.MachineName = "worker-machine";
        record.ProcessStartedUtc = ProcessStartedUtc;
        record.RunningSinceUtc = RunningSinceUtc;

        var result = _mapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Forbidden field");
    }

    [Fact]
    public void ToDomain_WhenCanceledRecordHasLastHeartbeatUtc_ShouldFail()
    {
        var record = ValidCanceledRecord();
        record.LastHeartbeatUtc = LastHeartbeatUtc;

        var result = _mapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("LastHeartbeatUtc");
    }

    [Fact]
    public void ToDomain_WhenCanceledRecordHasProcessFields_ShouldFail()
    {
        var record = ValidCanceledRecord();
        record.ProcessId = 123;
        record.Hostname = "worker-host";
        record.MachineName = "worker-machine";
        record.ProcessStartedUtc = ProcessStartedUtc;
        record.RunningSinceUtc = RunningSinceUtc;

        var result = _mapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("Forbidden field");
    }

    [Fact]
    public void ApplyToRecord_WhenRuntimeIsPending_ShouldSetStatus()
    {
        var record = new WorkerRecord();

        var runtime = new WorkerRuntime.Pending();

        var result = _mapper.ApplyToRecord(runtime, record);

        result.IsSuccess.Should().BeTrue();

        record.StatusCode.Should().Be(WorkerStatusCode.Pending);
    }

    [Fact]
    public void ApplyToRecord_WhenRuntimeIsStarting_ShouldSetStartingFields()
    {
        var record = new WorkerRecord();

        var runtime = new WorkerRuntime.Starting(
            ProcessId.From(123),
           "worker-host",
            "worker-machine",
            Option.FromNullable<DateTimeOffset>(ProcessStartedUtc),
            Option.FromNullable<DateTimeOffset>(LastHeartbeatUtc),
            1,
            3);

        var result = _mapper.ApplyToRecord(runtime, record);

        result.IsSuccess.Should().BeTrue();

        record.StatusCode.Should().Be(WorkerStatusCode.Starting);
        record.ProcessId.Should().Be(123);
        record.Hostname.Should().Be("worker-host");
        record.MachineName.Should().Be("worker-machine");
        record.ProcessStartedUtc.Should().Be(ProcessStartedUtc);
        record.StartupAttemptCount.Should().Be(1);
        record.MaxStartupAttemptCount.Should().Be(3);
    }

    [Fact]
    public void ApplyToRecord_WhenRuntimeIsRunning_ShouldSetRunningFields()
    {
        var record = new WorkerRecord();

        var runtime = new WorkerRuntime.Running(
            ProcessId.From(123),
            "worker-host",
            "worker-machine",
            Option.FromNullable<DateTimeOffset>(ProcessStartedUtc),
            Option.FromNullable<DateTimeOffset>(RunningSinceUtc),
            Option.FromNullable<DateTimeOffset>(LastHeartbeatUtc),
            1,
            3);

        var result = _mapper.ApplyToRecord(runtime, record);

        result.IsSuccess.Should().BeTrue();

        record.StatusCode.Should().Be(WorkerStatusCode.Running);
        record.ProcessId.Should().Be(123);
        record.Hostname.Should().Be("worker-host");
        record.MachineName.Should().Be("worker-machine");
        record.ProcessStartedUtc.Should().Be(ProcessStartedUtc);
        record.RunningSinceUtc.Should().Be(RunningSinceUtc);
        record.LastHeartbeatUtc.Should().Be(LastHeartbeatUtc);
        record.StartupAttemptCount.Should().Be(1);
        record.MaxStartupAttemptCount.Should().Be(3);
    }

    [Fact]
    public void ApplyToRecord_WhenRuntimeIsFailed_ShouldSetFailedFields()
    {
        var record = new WorkerRecord();

        var runtime = new WorkerRuntime.Failed(
            2,
            3,
            LastHeartbeatUtc);

        var result = _mapper.ApplyToRecord(runtime, record);

        result.IsSuccess.Should().BeTrue();

        record.StatusCode.Should().Be(WorkerStatusCode.Failed);
        record.LastHeartbeatUtc.Should().Be(LastHeartbeatUtc);
        record.StartupAttemptCount.Should().Be(2);
        record.MaxStartupAttemptCount.Should().Be(3);
    }

    [Fact]
    public void ApplyToRecord_WhenRuntimeIsCanceled_ShouldSetCanceledFields()
    {
        var record = new WorkerRecord();

        var runtime = new WorkerRuntime.Canceled(
            1,
            3);

        var result = _mapper.ApplyToRecord(runtime, record);

        result.IsSuccess.Should().BeTrue();

        record.StatusCode.Should().Be(WorkerStatusCode.Canceled);
        record.StartupAttemptCount.Should().Be(1);
        record.MaxStartupAttemptCount.Should().Be(3);

        record.ProcessId.Should().BeNull();
        record.Hostname.Should().BeNull();
        record.MachineName.Should().BeNull();
        record.ProcessStartedUtc.Should().BeNull();
        record.RunningSinceUtc.Should().BeNull();
        record.LastHeartbeatUtc.Should().BeNull();
    }

    [Fact]
    public void ApplyToRecord_WhenApplyingPendingToPreviouslyRunningRecord_ShouldClearRuntimeFields()
    {
        var record = ValidRunningRecord();

        var result = _mapper.ApplyToRecord(new WorkerRuntime.Pending(), record);

        result.IsSuccess.Should().BeTrue();

        record.StatusCode.Should().Be(WorkerStatusCode.Pending);

        record.ProcessId.Should().BeNull();
        record.Hostname.Should().BeNull();
        record.MachineName.Should().BeNull();
        record.ProcessStartedUtc.Should().BeNull();
        record.RunningSinceUtc.Should().BeNull();
        record.LastHeartbeatUtc.Should().BeNull();

        record.StartupAttemptCount.Should().Be(0);
        record.MaxStartupAttemptCount.Should().Be(0);
    }

    [Fact]
    public void ApplyToRecord_WhenApplyingStartingToPreviouslyRunningRecord_ShouldClearRunningOnlyFields()
    {
        var record = ValidRunningRecord();

        var runtime = new WorkerRuntime.Starting(
            ProcessId.From(456),
            "new-host",
            "new-machine",
            Option.FromNullable<DateTimeOffset>(ProcessStartedUtc),
            Option.FromNullable<DateTimeOffset>(LastHeartbeatUtc),
            2,
            3);

        var result = _mapper.ApplyToRecord(runtime, record);

        result.IsSuccess.Should().BeTrue();

        record.StatusCode.Should().Be(WorkerStatusCode.Starting);
        record.ProcessId.Should().Be(456);
        record.Hostname.Should().Be("new-host");
        record.MachineName.Should().Be("new-machine");
        record.ProcessStartedUtc.Should().Be(ProcessStartedUtc);
        record.StartupAttemptCount.Should().Be(2);
        record.MaxStartupAttemptCount.Should().Be(3);

        record.RunningSinceUtc.Should().BeNull();
        record.LastHeartbeatUtc.Should().BeNull();
        
    }

    [Fact]
    public void ApplyToRecord_WhenApplyingFailedToPreviouslyRunningRecord_ShouldClearProcessFields()
    {
        var record = ValidRunningRecord();

        var runtime = new WorkerRuntime.Failed(
            2,
            3,
            LastHeartbeatUtc);

        var result = _mapper.ApplyToRecord(runtime, record);

        result.IsSuccess.Should().BeTrue();

        record.StatusCode.Should().Be(WorkerStatusCode.Failed);
        record.LastHeartbeatUtc.Should().Be(LastHeartbeatUtc);
        record.StartupAttemptCount.Should().Be(2);
        record.MaxStartupAttemptCount.Should().Be(3);

        record.ProcessId.Should().BeNull();
        record.Hostname.Should().BeNull();
        record.MachineName.Should().BeNull();
        record.ProcessStartedUtc.Should().BeNull();
        record.RunningSinceUtc.Should().BeNull();
    }

    [Fact]
    public void ApplyToRecord_WhenApplyingCanceledToPreviouslyRunningRecord_ShouldClearProcessAndHeartbeatFields()
    {
        var record = ValidRunningRecord();

        var runtime = new WorkerRuntime.Canceled(
            1,
            3);

        var result = _mapper.ApplyToRecord(runtime, record);

        result.IsSuccess.Should().BeTrue();

        record.StatusCode.Should().Be(WorkerStatusCode.Canceled);
        record.StartupAttemptCount.Should().Be(1);
        record.MaxStartupAttemptCount.Should().Be(3);

        record.ProcessId.Should().BeNull();
        record.Hostname.Should().BeNull();
        record.MachineName.Should().BeNull();
        record.ProcessStartedUtc.Should().BeNull();
        record.RunningSinceUtc.Should().BeNull();
        record.LastHeartbeatUtc.Should().BeNull();
    }
    
    private static WorkerRecord ValidStartingRecord()
    {
        return new WorkerRecord
        {
            StatusCode = WorkerStatusCode.Starting,
            ProcessId = 123,
            Hostname = "worker-host",
            MachineName = "worker-machine",
            ProcessStartedUtc = ProcessStartedUtc,
            StartupAttemptCount = 1,
            MaxStartupAttemptCount = 3
        };
    }

    private static WorkerRecord ValidRunningRecord()
    {
        return new WorkerRecord
        {
            StatusCode = WorkerStatusCode.Running,
            ProcessId = 123,
            Hostname = "worker-host",
            MachineName = "worker-machine",
            ProcessStartedUtc = ProcessStartedUtc,
            RunningSinceUtc = RunningSinceUtc,
            LastHeartbeatUtc = LastHeartbeatUtc,
            StartupAttemptCount = 1,
            MaxStartupAttemptCount = 3
        };
    }

    private static WorkerRecord ValidFailedRecord()
    {
        return new WorkerRecord
        {
            StatusCode = WorkerStatusCode.Failed,
            LastHeartbeatUtc = LastHeartbeatUtc,
            StartupAttemptCount = 2,
            MaxStartupAttemptCount = 3
        };
    }

    private static WorkerRecord ValidCanceledRecord()
    {
        return new WorkerRecord
        {
            StatusCode = WorkerStatusCode.Canceled,
            StartupAttemptCount = 1,
            MaxStartupAttemptCount = 3
        };
    }
}