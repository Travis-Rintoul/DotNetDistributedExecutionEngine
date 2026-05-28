using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Jobs.Execution;

public class ProcessScheduledJobCommand : ICommand<Result<JobExecutionResult, JobExecutionError>>
{
    
}