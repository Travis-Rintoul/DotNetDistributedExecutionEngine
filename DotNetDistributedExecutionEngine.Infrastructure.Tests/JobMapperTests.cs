using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobLeasing;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobStatuses;
using FluentAssertions;

namespace DotNetDistributedExecutionEngine.Infrastructure.Tests;

public sealed class JobMapperTests
{
    private readonly IJobMapper _jobMapper;
    private readonly IJobLeaseMapper _leaseMapper;
    private readonly IJobStatusMapper _jobStatusMapper;

    public JobMapperTests()
    {
        _leaseMapper = new JobLeaseMapper();
        _jobStatusMapper = new JobStatusMapper();
        _jobMapper = new JobMapper(_jobStatusMapper, _leaseMapper);
    }

    [Fact]
    public void ToDomain_WhenRecordIsValidPendingAvailableJob_ReturnsDomainJob()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var createdUtc = new DateTimeOffset(2026, 6, 6, 10, 0, 0, TimeSpan.Zero);

        var record = new JobRecord
        {
            JobId = jobId,
            JobTypeCode = "EMAIL",
            PayloadJson = """{"to":"test@example.com"}""",
            StatusCode = JobStatusCode.Pending,
            CreatedUtc = createdUtc,
            AttemptsCount = 1,
            MaxAttemptsCount = 3,

            // Lease fields
            LeasedUtc = null,
            AssignedWorkerId = null
        };

        // Act
        var result = _jobMapper.ToDomain(record);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.JobId.Value.Should().Be(jobId);
        result.Value.JobType.Should().Be("EMAIL");
        result.Value.PayloadJson.Should().Be("""{"to":"test@example.com"}""");
        result.Value.CreatedUtc.Should().Be(createdUtc);
        result.Value.AttemptsCount.Should().Be(1);
        result.Value.MaxAttemptsCount.Should().Be(3);

        result.Value.Status.Should().BeOfType<JobStatus.Pending>();
        result.Value.Lease.Should().BeOfType<JobLease.Available>();
    }

    [Fact]
    public void ToDomain_WhenRecordHasRunningStatus_ReturnsRunningJob()
    {
        // Arrange
        var startedUtc = new DateTimeOffset(2026, 6, 6, 10, 5, 0, TimeSpan.Zero);

        var record = CreateValidPendingRecord();
        record.StatusCode = JobStatusCode.Running;
        record.StartedUtc = startedUtc;

        // Act
        var result = _jobMapper.ToDomain(record);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var status = result.Value.Status.Should().BeOfType<JobStatus.Running>().Subject;
        status.StartedUtc.Should().Be(startedUtc);
    }

    [Fact]
    public void ToPersistence_WhenJobIsPendingAndAvailable_ReturnsRecord()
    {
        // Arrange
        var createdUtc = new DateTimeOffset(2026, 6, 6, 10, 0, 0, TimeSpan.Zero);

        var job = Job.Rehydrate(
            jobId: Guid.NewGuid(),
            jobType: "IMPORT",
            payloadJson: """{"file":"transactions.csv"}""",
            status: new JobStatus.Pending(),
            lease: new JobLease.Available(),
            createdUtc: createdUtc,
            attemptsCount: 2,
            maxAttemptsCount: 5,
            assignedWorkerId: Option<WorkerId>.None);

        // Act
        var record = _jobMapper.ToPersistence(job);

        // Assert
        record.JobId.Should().Be(job.JobId.Value);
        record.JobTypeCode.Should().Be("IMPORT");
        record.PayloadJson.Should().Be("""{"file":"transactions.csv"}""");
        record.CreatedUtc.Should().Be(createdUtc);
        record.StatusCode.Should().Be(JobStatusCode.Pending);
        record.AttemptsCount.Should().Be(2);
        record.MaxAttemptsCount.Should().Be(5);
    }

    [Fact]
    public void ApplyToRecord_WhenRecordHasExistingValues_OverwritesMappedFields()
    {
        // Arrange
        var createdUtc = new DateTimeOffset(2026, 6, 6, 10, 0, 0, TimeSpan.Zero);

        var job = Job.Rehydrate(
            jobId: Guid.NewGuid(),
            jobType: "EXPORT",
            payloadJson: null,
            status: new JobStatus.Pending(),
            lease: new JobLease.Available(),
            createdUtc: createdUtc,
            attemptsCount: 0,
            maxAttemptsCount: 3,
            assignedWorkerId: Option<WorkerId>.None);

        var record = new JobRecord
        {
            JobId = Guid.NewGuid(),
            JobTypeCode = "OLD",
            PayloadJson = "OLD",
            CreatedUtc = DateTimeOffset.MinValue,
            StatusCode = JobStatusCode.Failed,
            AttemptsCount = 99,
            MaxAttemptsCount = 99
        };

        // Act
        _jobMapper.ApplyToRecord(job, record);

        // Assert
        record.JobId.Should().Be(job.JobId.Value);
        record.JobTypeCode.Should().Be("EXPORT");
        record.PayloadJson.Should().BeNull();
        record.CreatedUtc.Should().Be(createdUtc);
        record.StatusCode.Should().Be(JobStatusCode.Pending);
        record.AttemptsCount.Should().Be(0);
        record.MaxAttemptsCount.Should().Be(3);
    }

    private static JobRecord CreateValidPendingRecord()
    {
        return new JobRecord
        {
            JobId = Guid.NewGuid(),
            JobTypeCode = "GENERIC",
            PayloadJson = "{}",
            StatusCode = JobStatusCode.Pending,
            CreatedUtc = new DateTimeOffset(2026, 6, 6, 10, 0, 0, TimeSpan.Zero),
            AttemptsCount = 0,
            MaxAttemptsCount = 3,

            LeasedUtc = null,
            AssignedWorkerId = null
        };
    }
}