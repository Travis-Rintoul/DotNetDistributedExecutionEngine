using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Jobs.Leasing;

public class LeaseJobForWorkerCommandHandler(IJobLeaseStore store) : ICommandHandler<LeaseJobForWorkerCommand, Option<JobWorkerLease>>
{
    public async Task<Option<JobWorkerLease>> HandleAsync(LeaseJobForWorkerCommand command, CancellationToken token = default)
        => await store.TryLeaseNextJobAsync(command.WorkerId, token);
}