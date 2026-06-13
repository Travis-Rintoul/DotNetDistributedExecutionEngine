using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Jobs.Persistence;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Jobs.Scheduling;

public class ScheduleJobCommandHandler(
    IUnitOfWork uow,
    IJobRepository jobRepository
) : ICommandHandler<ScheduleJobCommand, Result<JobId, ScheduleJobError>>
{
    public async Task<Result<JobId, ScheduleJobError>> HandleAsync(ScheduleJobCommand command, CancellationToken token = default)
    {
        await jobRepository.AddAsync(command.Job, token);
        await uow.SaveChangesAsync(token);

        return Result.Success<JobId, ScheduleJobError>(command.Job.JobId);
    }
}