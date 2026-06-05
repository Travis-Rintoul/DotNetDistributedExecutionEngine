using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobStatuses;
using FluentAssertions;

namespace DotNetDistributedExecutionEngine.Infrastructure.Tests;

public sealed class JobStatusMapperTests
{
    private readonly IJobStatusMapper _JobStatusMapper = new JobStatusMapper();
    
    [Theory]
    [MemberData(nameof(StatusCodeCases))]
    public void ToCode_WhenStatusIsKnown_ReturnsExpectedCode(
        JobStatus status,
        JobStatusCode expectedCode)
    {
        _JobStatusMapper.ToCode(status).Should().Be(expectedCode);
    }

    [Fact]
    public void MapStatus_WhenPending_ReturnsPendingStatus()
    {
        var record = ValidRecord();
        record.StatusCode = JobStatusCode.Pending;

        var result = _JobStatusMapper.Map(record);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<JobStatus.Pending>();
    }

    [Fact]
    public void MapStatus_WhenRunningHasStartedUtc_ReturnsRunningStatus()
    {
        var startedUtc = new DateTimeOffset(2026, 6, 3, 10, 0, 0, TimeSpan.Zero);

        var record = ValidRecord();
        record.StatusCode = JobStatusCode.Running;
        record.StartedUtc = startedUtc;

        var result = _JobStatusMapper.Map(record);

        result.IsSuccess.Should().BeTrue();

        var status = result.Value.Should().BeOfType<JobStatus.Running>().Subject;
        status.StartedUtc.Should().Be(startedUtc);
    }

    [Fact]
    public void MapStatus_WhenRunningMissingStartedUtc_ReturnsFailure()
    {
        var record = ValidRecord();
        record.StatusCode = JobStatusCode.Running;
        record.StartedUtc = null;

        var result = _JobStatusMapper.Map(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Running job must have StartedUtc.");
    }

    [Fact]
    public void MapStatus_WhenSucceededHasStartedUtcAndCompletedUtc_ReturnsSucceededStatus()
    {
        var startedUtc = new DateTimeOffset(2026, 6, 3, 10, 0, 0, TimeSpan.Zero);
        var completedUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);

        var record = ValidRecord();
        record.StatusCode = JobStatusCode.Succeeded;
        record.StartedUtc = startedUtc;
        record.CompletedUtc = completedUtc;

        var result = _JobStatusMapper.Map(record);

        result.IsSuccess.Should().BeTrue();

        var status = result.Value.Should().BeOfType<JobStatus.Succeeded>().Subject;
        status.StartedUtc.Should().Be(startedUtc);
        status.CompletedUtc.Should().Be(completedUtc);
    }

    [Fact]
    public void MapStatus_WhenSucceededMissingStartedUtc_ReturnsFailure()
    {
        var record = ValidRecord();
        record.StatusCode = JobStatusCode.Succeeded;
        record.StartedUtc = null;
        record.CompletedUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);

        var result = _JobStatusMapper.Map(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Succeeded job must have StartedUtc.");
    }

    [Fact]
    public void MapStatus_WhenSucceededMissingCompletedUtc_ReturnsFailure()
    {
        var record = ValidRecord();
        record.StatusCode = JobStatusCode.Succeeded;
        record.StartedUtc = new DateTimeOffset(2026, 6, 3, 10, 0, 0, TimeSpan.Zero);
        record.CompletedUtc = null;

        var result = _JobStatusMapper.Map(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Succeeded job must have CompletedUtc.");
    }

    [Fact]
    public void MapStatus_WhenFailedHasFailedUtcAndReason_ReturnsFailedStatus()
    {
        var failedUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);

        var record = ValidRecord();
        record.StatusCode = JobStatusCode.Failed;
        record.FailedUtc = failedUtc;
        record.FailureReason = "Worker crashed.";

        var result = _JobStatusMapper.Map(record);

        result.IsSuccess.Should().BeTrue();

        var status = result.Value.Should().BeOfType<JobStatus.Failed>().Subject;
        status.FailedUtc.Should().Be(failedUtc);
        status.Reason.Should().Be("Worker crashed.");
    }

    [Fact]
    public void MapStatus_WhenFailedHasNullFailureReason_UsesEmptyString()
    {
        var failedUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);

        var record = ValidRecord();
        record.StatusCode = JobStatusCode.Failed;
        record.FailedUtc = failedUtc;
        record.FailureReason = null;

        var result = _JobStatusMapper.Map(record);

        result.IsSuccess.Should().BeTrue();

        var status = result.Value.Should().BeOfType<JobStatus.Failed>().Subject;
        status.FailedUtc.Should().Be(failedUtc);
        status.Reason.Should().Be(string.Empty);
    }

    [Fact]
    public void MapStatus_WhenFailedMissingFailedUtc_ReturnsFailure()
    {
        var record = ValidRecord();
        record.StatusCode = JobStatusCode.Failed;
        record.FailedUtc = null;
        record.FailureReason = "Worker crashed.";

        var result = _JobStatusMapper.Map(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Failed job must have FailedUtc.");
    }

    public static TheoryData<JobStatus, JobStatusCode> StatusCodeCases()
    {
        var startedUtc = new DateTimeOffset(2026, 6, 3, 10, 0, 0, TimeSpan.Zero);
        var completedUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);
        var failedUtc = new DateTimeOffset(2026, 6, 3, 10, 5, 0, TimeSpan.Zero);

        return new TheoryData<JobStatus, JobStatusCode>
        {
            { new JobStatus.Pending(), JobStatusCode.Pending },
            { new JobStatus.Running(startedUtc), JobStatusCode.Running },
            { new JobStatus.Succeeded(startedUtc, completedUtc), JobStatusCode.Succeeded },
            { new JobStatus.Failed(failedUtc, "Worker crashed."), JobStatusCode.Failed }
        };
    }

    private static JobRecord ValidRecord()
    {
        return new JobRecord
        {
            Id = 123,
            JobId = Guid.NewGuid(),
            JobTypeCode = "TEST_JOB",
            PayloadJson = null,
            StatusCode = JobStatusCode.Pending,
            CreatedUtc = new DateTimeOffset(2026, 6, 3, 9, 0, 0, TimeSpan.Zero),
            LeasedUtc = null,
            AssignedWorkerId = null,
            AttemptsCount = 0,
            MaxAttemptsCount = 3,
            StartedUtc = null,
            CompletedUtc = null,
            FailedUtc = null,
            FailureReason = null
        };
    }
}