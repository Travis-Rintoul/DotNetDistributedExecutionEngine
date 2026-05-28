using DistributedExecutionEngine.Application.Features.Jobs.Execution;

namespace DistributedExecutionEngine.Application.Features.Jobs.Scheduling;

public interface IJobScheduler
{
    public Task ScheduleJobAsync<T>(IScheduledJobPayload<T> payload);
}