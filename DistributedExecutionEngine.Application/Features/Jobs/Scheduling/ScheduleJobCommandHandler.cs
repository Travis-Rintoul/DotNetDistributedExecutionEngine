using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Jobs.Scheduling;

public class ScheduleJobCommandHandler : ICommandHandler<ScheduleJobCommand, Result<JobId, ScheduleJobError>>
{
    public Task<Result<JobId, ScheduleJobError>> HandleAsync(ScheduleJobCommand command, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}