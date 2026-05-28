using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.State;

public class MarkWorkerRunningCommand : ICommand<Result<Unit, MarkWorkerRunningError>>
{
    
}