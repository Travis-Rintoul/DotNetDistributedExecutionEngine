using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Features.Jobs.Leasing;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;
using IJobLeaseStore = DistributedExecutionEngine.Application.Abstractions.IJobLeaseStore;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Jobs;

public class JobLeaseStore : IJobLeaseStore
{
    public Task<Option<JobLease>> TryLeaseNextJobAsync(WorkerId workerId, IClock instant, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}