using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerLeases;
using DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerStatuses;
using FluentAssertions;

namespace DotNetDistributedExecutionEngine.Infrastructure.Tests;

public sealed class WorkerMapperTests
{
    private readonly IWorkerMapper _workerMapper;
    

    public WorkerMapperTests()
    {
        var workerStatusMapper = new WorkerStatusMapper();
        var workerLeaseMapper = new WorkerLeaseMapper();

        _workerMapper = new WorkerMapper(workerStatusMapper, workerLeaseMapper);
    }

    [Fact]
    public void ToDomain_WhenRecordIsValid_ShouldMapWorkerFields()
    {
        var workerId = Guid.NewGuid();
        var createdUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);

        var record = new WorkerRecord
        {
            WorkerId = workerId,
            Hostname = "worker-01",
            MaxConcurrency = 8,
            CreatedUtc = createdUtc,
            StatusCode = WorkerStatusCode.Pending
        };

        var result = _workerMapper.ToDomain(record);

        result.IsSuccess.Should().BeTrue();

        var worker = result.Value;

        worker.WorkerId.Value.Should().Be(workerId);
        worker.Hostname.Should().Be("worker-01");
        worker.MaxConcurrency.Should().Be(8);
        worker.CreatedUtc.Should().Be(createdUtc);
        worker.Status.Should().BeOfType<WorkerStatus.Pending>();
    }

    [Fact]
    public void ToDomain_WhenStatusMappingFails_ShouldReturnFailure()
    {
        var record = new WorkerRecord
        {
            WorkerId = Guid.NewGuid(),
            Hostname = "worker-01",
            MaxConcurrency = 8,
            CreatedUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero),
            StatusCode = WorkerStatusCode.Failed,

            // Starting requires these fields.
            FailedUtc = null,
            FailureReason = null
        };

        var result = _workerMapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void ToPersistence_WhenWorkerIsPending_ShouldMapWorkerFieldsAndStatusFields()
    {
        var workerId = Guid.NewGuid();
        var createdUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);

        var worker = Worker.Rehydrate(
            workerId: workerId,
            hostname: "worker-01",
            status: new WorkerStatus.Pending(),
            lease: new WorkerLease.Unclaimed(),
            maxConcurrency: 8,
            createdUtc: createdUtc);

        var record = _workerMapper.ToPersistence(worker);

        record.WorkerId.Should().Be(workerId);
        record.Hostname.Should().Be("worker-01");
        record.MaxConcurrency.Should().Be(8);
        record.CreatedUtc.Should().Be(createdUtc);

        record.StatusCode.Should().Be(WorkerStatusCode.Pending);

        record.ClaimedUtc.Should().BeNull();
        record.SupervisorId.Should().BeNull();
        record.StartedUtc.Should().BeNull();
        record.FailedUtc.Should().BeNull();
        record.FailureReason.Should().BeNull();
        record.StoppedUtc.Should().BeNull();
    }

    [Fact]
    public void ApplyToRecord_WhenWorkerIsPending_ShouldMapWorkerFieldsAndStatusFields()
    {
        var workerId = Guid.NewGuid();
        var createdUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);

        var worker = Worker.Rehydrate(
            workerId: workerId,
            hostname: "worker-01",
            status: new WorkerStatus.Pending(),
            lease: new WorkerLease.Unclaimed(),
            maxConcurrency: 8,
            createdUtc: createdUtc);

        var record = DirtyRecord();

        _workerMapper.ApplyToRecord(worker, record);

        record.WorkerId.Should().Be(workerId);
        record.Hostname.Should().Be("worker-01");
        record.MaxConcurrency.Should().Be(8);
        record.CreatedUtc.Should().Be(createdUtc);

        record.StatusCode.Should().Be(WorkerStatusCode.Pending);

        record.ClaimedUtc.Should().BeNull();
        record.SupervisorId.Should().BeNull();
        record.StartedUtc.Should().BeNull();
        record.FailedUtc.Should().BeNull();
        record.FailureReason.Should().BeNull();
        record.StoppedUtc.Should().BeNull();
    }

    [Fact]
    public void ApplyToRecord_WhenWorkerIsFailed_ShouldMapWorkerFieldsAndFailedStatusFields()
    {
        var workerId = Guid.NewGuid();
        var createdUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);
        var failedUtc = new DateTimeOffset(2026, 6, 3, 10, 10, 0, TimeSpan.Zero);

        var worker = Worker.Rehydrate(
            workerId: workerId,
            hostname: "worker-01",
            status: new WorkerStatus.Failed(failedUtc, "worker crashed"),
            lease: new WorkerLease.Unclaimed(),
            maxConcurrency: 8,
            createdUtc: createdUtc);

        var record = DirtyRecord();

        _workerMapper.ApplyToRecord(worker, record);

        record.WorkerId.Should().Be(workerId);
        record.Hostname.Should().Be("worker-01");
        record.MaxConcurrency.Should().Be(8);
        record.CreatedUtc.Should().Be(createdUtc);

        record.StatusCode.Should().Be(WorkerStatusCode.Failed);

        record.FailedUtc.Should().Be(failedUtc);
        record.FailureReason.Should().Be("worker crashed");

        record.ClaimedUtc.Should().BeNull();
        record.SupervisorId.Should().BeNull();
        record.StartedUtc.Should().BeNull();
        record.StoppedUtc.Should().BeNull();
    }

    private static WorkerRecord DirtyRecord()
    {
        return new WorkerRecord
        {
            WorkerId = Guid.NewGuid(),
            Hostname = "dirty-worker",
            MaxConcurrency = 99,
            CreatedUtc = new DateTimeOffset(2020, 1, 1, 0, 0, 0, TimeSpan.Zero),

            StatusCode = WorkerStatusCode.Running,
            ClaimedUtc = new DateTimeOffset(2026, 6, 1, 10, 0, 0, TimeSpan.Zero),
            SupervisorId = 123,
            StartedUtc = new DateTimeOffset(2026, 6, 1, 10, 1, 0, TimeSpan.Zero),
            FailedUtc = new DateTimeOffset(2026, 6, 1, 10, 2, 0, TimeSpan.Zero),
            FailureReason = "old failure",
            StoppedUtc = new DateTimeOffset(2026, 6, 1, 10, 3, 0, TimeSpan.Zero)
        };
    }
}