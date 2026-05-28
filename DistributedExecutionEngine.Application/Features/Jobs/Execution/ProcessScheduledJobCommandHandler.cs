using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Jobs.Execution;

public class ProcessScheduledJobCommandHandler: ICommandHandler<ProcessScheduledJobCommand, Result<JobExecutionResult, JobExecutionError>>
{
    public Task<Result<JobExecutionResult, JobExecutionError>> HandleAsync(ProcessScheduledJobCommand command)
    {
        throw new NotImplementedException();
    }
}