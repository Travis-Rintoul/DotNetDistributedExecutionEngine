using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Jobs.Leasing;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DistributedExecutionEngine.WorkerHost;

public sealed class WorkerOptions
{
    public WorkerId WorkerId { get; set; }
}

public sealed class WorkerBackgroundService(IServiceScopeFactory scopeFactory, ILogger<WorkerBackgroundService> logger, IOptions<WorkerOptions> options) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        var workerId = options.Value.WorkerId;
        using var scope = scopeFactory.CreateScope();
        var dispatcher = scope.ServiceProvider.GetRequiredService<ICommandDispatcher>();

        logger.LogInformation("[Worker ({WorkerId})] Starting", workerId);

        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), token);
                logger.LogInformation("[Worker ({ValueWorkerId})] Worker did work", options.Value.WorkerId );

                var jobLeaseOption = await dispatcher.SendAsync(new LeaseJobForWorkerCommand(options.Value.WorkerId), token);
                if (jobLeaseOption.IsSome)
                {
                    Console.WriteLine(jobLeaseOption.Value);
                }
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
            // Normal shutdown.
        }
        finally
        {
            logger.LogInformation("[Worker ({WorkerId})] Stopping", workerId);
        }
    }
}