using System.Windows.Input;
using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Health;

public class RecordWorkerHeartbeatCommand : ICommand<Result<Unit, WorkerHeartbeatError>>
{
    
}