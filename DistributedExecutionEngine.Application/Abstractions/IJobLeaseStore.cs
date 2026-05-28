using DistributedExecutionEngine.Application.Features.Jobs.Leasing;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Abstractions;

public interface IJobLeaseStore
{
    Task<Option<JobLease>> TryLeaseNextJobAsync(WorkerId workerId, IClock instant, CancellationToken cancellationToken);
}