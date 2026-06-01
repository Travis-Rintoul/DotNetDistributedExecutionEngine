using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Abstractions.Persistence;
using DistributedExecutionEngine.Application.Features.Workers.Supervision;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Lifecycle;

public class LaunchWorkerCommandHandler(
    IClock clock, 
    IWorkerRepository workerRepository,
    IWorkerProcessLauncher workerProcessLauncher
) : ICommandHandler<LaunchWorkerCommand, Result<Unit, string>>
{
    public async Task<Result<Unit, string>> HandleAsync(LaunchWorkerCommand command, CancellationToken token)
    {
        Option<Worker> workerOption = await workerRepository.FindByKeyAsync(command.WorkerId, token);
        if (workerOption.IsNone)
            return Result<Unit, string>.Failure("Worker not found");
        
        var worker = workerOption.Value;
        var now = clock.UtcNow;

        var markStarting = worker.MarkStarting();
        if (markStarting.IsFailure)
            return Result.Failure(markStarting.Error.ToString());

        await workerProcessLauncher.LaunchAsync(worker.WorkerId, token);
        
        var markRunning = worker.MarkRunning();
        if (markRunning.IsFailure)
            return Result.Failure(markRunning.Error.ToString());

        return Result.Success<string>();
    }
}