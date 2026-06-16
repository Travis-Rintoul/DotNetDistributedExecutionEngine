using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Abstractions.Persistence;
using DistributedExecutionEngine.Application.Features.Workers.Process;
using DistributedExecutionEngine.Application.Features.Workers.Supervision;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Lifecycle;

public class LaunchWorkerCommandHandler(
    IWorkerRepository workerRepository,
    IWorkerProcessLauncher workerProcessLauncher
) : ICommandHandler<LaunchWorkerCommand, Result<ProcessId, string>>
{
    public async Task<Result<ProcessId, string>> HandleAsync(LaunchWorkerCommand command, CancellationToken token)
    {
        Option<Worker> workerOption = await workerRepository.FindByKeyAsync(command.WorkerId, token);
        if (workerOption.IsNone)
            return Result<ProcessId, string>.Failure("Worker not found");
        
        var worker = workerOption.Value;
        var launchResult = await workerProcessLauncher.LaunchAsync(worker.WorkerId, token);
        if (launchResult.IsFailure)
            return Result<ProcessId, string>.Failure(launchResult.Error);
        
        var markRunning = worker.MarkRunning();
        if (markRunning.IsFailure)
            return Result<ProcessId, string>.Failure(markRunning.Error);

        return Result<ProcessId, string>.Success(launchResult.Value);
    }
}