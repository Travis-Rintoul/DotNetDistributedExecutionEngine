using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Jobs.Leasing;

public class LeaseJobCommandHandler : ICommandHandler<LeaseJobCommand, Option<JobLease>>
{
    public Task<Option<JobLease>> HandleAsync(LeaseJobCommand command)
    {
        return Task.FromResult(Option<JobLease>.None);
    }
}