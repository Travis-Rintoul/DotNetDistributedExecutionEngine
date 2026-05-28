using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Jobs.Leasing;

public interface IJobLeaseStore
{
    public Task<Option<JobLease>> TryLeaseNextJob(WorkerId jobId, CancellationToken token);
}