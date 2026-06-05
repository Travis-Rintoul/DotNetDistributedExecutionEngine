using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerStatuses;
using FluentAssertions;

namespace DotNetDistributedExecutionEngine.Infrastructure.Tests;

public sealed class WorkerStatusMapperTests
{
    private readonly IWorkerStatusMapper _mapper = new WorkerStatusMapper();

    [Theory]
    [MemberData(nameof(StatusCodeCases))]
    public void ToCode_WhenStatusIsKnown_ReturnsExpectedCode(
        WorkerStatus status,
        WorkerStatusCode expectedCode)
    {
        WorkerStatusCode result = _mapper.ToCode(status);

        result.Should().Be(expectedCode);
    }

    [Theory]
    [MemberData(nameof(MapCases))]
    public void Map_WhenRecordHasKnownStatusCode_ReturnsExpectedStatus(
        WorkerRecord record,
        Type expectedStatusType)
    {
        var result = _mapper.Map(record);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType(expectedStatusType);
    }

    [Fact]
    public void Map_WhenRecordIsFailedWithoutFailedUtc_ReturnsFailure()
    {
        var record = new WorkerRecord
        {
            StatusCode = WorkerStatusCode.Failed,
            FailedUtc = null,
            FailureReason = "Worker process crashed."
        };

        var result = _mapper.Map(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Map_WhenRecordIsCanceledWithoutCanceledUtc_ReturnsFailure()
    {
        var record = new WorkerRecord
        {
            StatusCode = WorkerStatusCode.Canceled,
            CanceledUtc = null,
            CancellationReason = "Worker was canceled."
        };

        var result = _mapper.Map(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Map_WhenRecordIsFailed_ReturnsFailedStatusWithDetails()
    {
        var failedUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);

        var record = new WorkerRecord
        {
            StatusCode = WorkerStatusCode.Failed,
            FailedUtc = failedUtc,
            FailureReason = "Worker process crashed."
        };

        var result = _mapper.Map(record);

        result.IsSuccess.Should().BeTrue();

        var status = result.Value.Should().BeOfType<WorkerStatus.Failed>().Subject;

        status.FailedUtc.Should().Be(failedUtc);
        status.FailReason.Should().Be("Worker process crashed.");
    }

    [Fact]
    public void Map_WhenRecordIsCanceled_ReturnsCanceledStatusWithDetails()
    {
        var canceledUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);

        var record = new WorkerRecord
        {
            StatusCode = WorkerStatusCode.Canceled,
            CanceledUtc = canceledUtc,
            CancellationReason = "Worker was lazy"
        };

        var result = _mapper.Map(record);

        result.IsSuccess.Should().BeTrue();

        var status = result.Value.Should().BeOfType<WorkerStatus.Canceled>().Subject;

        status.CanceledUtc.Should().Be(canceledUtc);
        status.CancelReason.Should().Be("Worker was lazy");
    }

    [Theory]
    [MemberData(nameof(ApplyCases))]
    public void ApplyToRecord_WhenStatusIsKnown_UpdatesStatusSpecificFields(
        WorkerStatus status,
        WorkerStatusCode expectedCode)
    {
        var record = DirtyRecord();

        _mapper.ApplyToRecord(status, record);

        _mapper.ToCode(status).Should().Be(expectedCode);
    }

    [Fact]
    public void ApplyToRecord_WhenStatusIsPending_ClearsStatusSpecificFields()
    {
        var record = DirtyRecord();

        _mapper.ApplyToRecord(new WorkerStatus.Pending(), record);

        record.FailedUtc.Should().BeNull();
        record.FailureReason.Should().BeNull();
        record.CanceledUtc.Should().BeNull();
        record.CancellationReason.Should().BeNull();
    }

    [Fact]
    public void ApplyToRecord_WhenStatusIsStarting_ClearsTerminalStatusFields()
    {
        var record = DirtyRecord();

        _mapper.ApplyToRecord(new WorkerStatus.Starting(), record);

        record.FailedUtc.Should().BeNull();
        record.FailureReason.Should().BeNull();
        record.CanceledUtc.Should().BeNull();
        record.CancellationReason.Should().BeNull();
    }

    [Fact]
    public void ApplyToRecord_WhenStatusIsRunning_ClearsTerminalStatusFields()
    {
        var record = DirtyRecord();

        _mapper.ApplyToRecord(new WorkerStatus.Running(), record);

        record.FailedUtc.Should().BeNull();
        record.FailureReason.Should().BeNull();
        record.CanceledUtc.Should().BeNull();
        record.CancellationReason.Should().BeNull();
    }

    [Fact]
    public void ApplyToRecord_WhenStatusIsFailed_SetsFailedFieldsAndClearsCanceledFields()
    {
        var failedUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);
        var record = DirtyRecord();

        _mapper.ApplyToRecord(
            new WorkerStatus.Failed(failedUtc, "Worker process crashed."),
            record);

        record.FailedUtc.Should().Be(failedUtc);
        record.FailureReason.Should().Be("Worker process crashed.");

        record.CanceledUtc.Should().BeNull();
        record.CancellationReason.Should().BeNull();
    }

    [Fact]
    public void ApplyToRecord_WhenStatusIsCanceled_SetsCanceledFieldsAndClearsFailedFields()
    {
        var canceledUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);
        var record = DirtyRecord();

        _mapper.ApplyToRecord(
            new WorkerStatus.Canceled(canceledUtc, "Worker was lazy"),
            record);

        record.CanceledUtc.Should().Be(canceledUtc);
        record.CancellationReason.Should().Be("Worker was lazy");

        record.FailedUtc.Should().BeNull();
        record.FailureReason.Should().BeNull();
    }

    public static TheoryData<WorkerStatus, WorkerStatusCode> StatusCodeCases()
    {
        var failedUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);
        var canceledUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);

        return new TheoryData<WorkerStatus, WorkerStatusCode>
        {
            { new WorkerStatus.Pending(), WorkerStatusCode.Pending },
            { new WorkerStatus.Starting(), WorkerStatusCode.Starting },
            { new WorkerStatus.Running(), WorkerStatusCode.Running },
            { new WorkerStatus.Failed(failedUtc, "Worker process crashed."), WorkerStatusCode.Failed },
            { new WorkerStatus.Canceled(canceledUtc, "Worker was lazy"), WorkerStatusCode.Canceled }
        };
    }

    public static TheoryData<WorkerRecord, Type> MapCases()
    {
        var failedUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);
        var canceledUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);

        return new TheoryData<WorkerRecord, Type>
        {
            {
                new WorkerRecord
                {
                    StatusCode = WorkerStatusCode.Pending
                },
                typeof(WorkerStatus.Pending)
            },
            {
                new WorkerRecord
                {
                    StatusCode = WorkerStatusCode.Starting
                },
                typeof(WorkerStatus.Starting)
            },
            {
                new WorkerRecord
                {
                    StatusCode = WorkerStatusCode.Running
                },
                typeof(WorkerStatus.Running)
            },
            {
                new WorkerRecord
                {
                    StatusCode = WorkerStatusCode.Failed,
                    FailedUtc = failedUtc,
                    FailureReason = "Worker process crashed."
                },
                typeof(WorkerStatus.Failed)
            },
            {
                new WorkerRecord
                {
                    StatusCode = WorkerStatusCode.Canceled,
                    CanceledUtc = canceledUtc,
                    CancellationReason = "Worker was lazy"
                },
                typeof(WorkerStatus.Canceled)
            }
        };
    }

    public static TheoryData<WorkerStatus, WorkerStatusCode> ApplyCases()
    {
        var failedUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);
        var canceledUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);

        return new TheoryData<WorkerStatus, WorkerStatusCode>
        {
            { new WorkerStatus.Pending(), WorkerStatusCode.Pending },
            { new WorkerStatus.Starting(), WorkerStatusCode.Starting },
            { new WorkerStatus.Running(), WorkerStatusCode.Running },
            { new WorkerStatus.Failed(failedUtc, "Worker process crashed."), WorkerStatusCode.Failed },
            { new WorkerStatus.Canceled(canceledUtc, "Worker was lazy"), WorkerStatusCode.Canceled }
        };
    }

    private static WorkerRecord DirtyRecord()
    {
        return new WorkerRecord
        {
            FailedUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero),
            FailureReason = "old failure",
            CanceledUtc = new DateTimeOffset(2026, 6, 3, 10, 10, 0, TimeSpan.Zero),
            CancellationReason = "old cancellation"
        };
    }
}