using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DistributedExecutionEngine.Application.Workers.Services;

public sealed class WorkerLauncherService(
    ILogger<WorkerLauncherService> logger,
    IConfiguration configuration
) : IWorkerLauncherService
{
    public Task StartWorkerAsync(CancellationToken token)
    {
        var workerPath = configuration["Worker:ExecutablePath"];
        
        var process = new Process();

        process.StartInfo = new ProcessStartInfo
        {
            FileName = workerPath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
                logger.LogInformation("[Worker] {Output}", e.Data);
        };
        
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
                logger.LogError("[Worker] {Output}", e.Data);
        };
        
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        
        logger.LogInformation("Worker started. PID {Pid}", process.Id);
        
        return Task.CompletedTask;
    }

    public Task StopWorkerAsync(int processId, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}