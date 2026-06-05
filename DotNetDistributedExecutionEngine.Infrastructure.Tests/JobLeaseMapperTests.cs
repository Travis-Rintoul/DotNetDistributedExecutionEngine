using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs;
using DistributedExecutionEngine.Infrastructure.Persistence.Jobs.JobLeasing;
using FluentAssertions;

namespace DotNetDistributedExecutionEngine.Infrastructure.Tests;

public class JobLeaseMapperTests
{
    private readonly IJobLeaseMapper _leaseMapper = new JobLeaseMapper();
    
    [Fact]
    public void ToDomain_WhenLeaseStatusIsAvailable_ReturnsAvailable()
    {
        var record = new JobRecord
        {
            LeaseStatusCode = JobLeaseStatusCode.Available
        };

        var result = _leaseMapper.ToDomain(record);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeOfType<JobLease.Available>();
    }

    [Fact]
    public void ToDomain_WhenLeaseStatusIsLeasedAndRequiredFieldsExist_ReturnsLeased()
    {
        var workerId = Guid.NewGuid();
        var leasedUtc = new DateTimeOffset(2026, 6, 6, 10, 0, 0, TimeSpan.Zero);

        var record = new JobRecord
        {
            LeaseStatusCode = JobLeaseStatusCode.Leased,
            LeasedUtc = leasedUtc,
            AssignedWorkerId = workerId
        };

        var result = _leaseMapper.ToDomain(record);

        result.IsSuccess.Should().BeTrue();

        var lease = result.Value.Should().BeOfType<JobLease.Leased>().Subject;
        lease.LeasedUtc.Should().Be(leasedUtc);
        lease.AssignedWorkerId.Value.Should().Be(workerId);
    }

    [Fact]
    public void ToDomain_WhenLeasedUtcIsMissing_ReturnsFailure()
    {
        var record = new JobRecord
        {
            LeaseStatusCode = JobLeaseStatusCode.Leased,
            LeasedUtc = null,
            AssignedWorkerId = Guid.NewGuid()
        };

        var result = _leaseMapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("LeasedUtc id is not assigned.");
    }

    [Fact]
    public void ToDomain_WhenAssignedWorkerIdIsMissing_ReturnsFailure()
    {
        var record = new JobRecord
        {
            LeaseStatusCode = JobLeaseStatusCode.Leased,
            LeasedUtc = new DateTimeOffset(2026, 6, 6, 10, 0, 0, TimeSpan.Zero),
            AssignedWorkerId = null
        };

        var result = _leaseMapper.ToDomain(record);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Worker id is not assigned.");
    }

    [Fact]
    public void ApplyToRecord_WhenLeaseIsAvailable_ClearsLeaseFields()
    {
        var record = new JobRecord
        {
            LeasedUtc = new DateTimeOffset(2026, 6, 6, 10, 0, 0, TimeSpan.Zero),
            AssignedWorkerId = Guid.NewGuid()
        };

        _leaseMapper.ApplyToRecord(new JobLease.Available(), record);

        record.LeasedUtc.Should().BeNull();
        record.AssignedWorkerId.Should().BeNull();
    }

    [Fact]
    public void ApplyToRecord_WhenLeaseIsLeased_WritesLeaseFields()
    {
        var workerId = WorkerId.From(Guid.NewGuid());
        var leasedUtc = new DateTimeOffset(2026, 6, 6, 10, 0, 0, TimeSpan.Zero);

        var record = new JobRecord();

        _leaseMapper.ApplyToRecord(new JobLease.Leased(leasedUtc, workerId), record);

        record.LeasedUtc.Should().Be(leasedUtc);
        record.AssignedWorkerId.Should().Be(workerId.Value);
    }
}