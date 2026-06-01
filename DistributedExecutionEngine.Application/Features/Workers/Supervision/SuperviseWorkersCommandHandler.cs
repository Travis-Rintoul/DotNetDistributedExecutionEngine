using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Workers.Persistence;
using DistributedExecutionEngine.Domain.Common;
using Microsoft.Extensions.Logging;

namespace DistributedExecutionEngine.Application.Features.Workers.Supervision;

public class SuperviseWorkersCommandHandler(
    ILogger<SuperviseWorkersCommandHandler> logger,
    IWorkerPoolStore workerPoolStore, 
    ICommandDispatcher commandDispatcher) : ICommandHandler<SuperviseWorkersCommand, Result<Unit, string>>
{
    public async Task<Result<Unit, string>> HandleAsync(SuperviseWorkersCommand command, CancellationToken token = default)
    {
        var workerClaim = await commandDispatcher.SendAsync(new ClaimPendingWorkerCommand(command.SupervisorId), token);
        if (workerClaim.IsFailure)
            return Result.Failure(workerClaim.Error);
        
        var workerOption = workerClaim.Value;
        if (workerOption.IsNone) 
            return Result.Success<string>();
        
        logger.LogInformation("Supervisor {CommandSupervisorId} claimed worker {WorkerOptionValue}", command.SupervisorId, workerOption.Value);
        
        return Result.Success<string>();
    }
    
}