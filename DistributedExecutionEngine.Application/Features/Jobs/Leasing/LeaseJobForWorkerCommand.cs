using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Jobs.Leasing;

public sealed record LeaseJobForWorkerCommand(WorkerId WorkerId) : ICommand<Option<JobWorkerLease>>
{
    
}