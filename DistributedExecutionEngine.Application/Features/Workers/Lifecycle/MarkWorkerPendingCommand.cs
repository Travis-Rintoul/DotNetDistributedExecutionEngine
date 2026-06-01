using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Lifecycle;

public class MarkWorkerPendingCommand : ICommand<Result<Unit, MarkWorkerPendingError>>
{
    
}