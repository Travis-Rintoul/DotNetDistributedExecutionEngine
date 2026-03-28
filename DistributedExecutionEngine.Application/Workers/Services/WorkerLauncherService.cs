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
        var workerDir = "/home/travis/Projects/DotNetDistributedExecutionEngine/DistributedExecutionEngine.WorkerHost/bin/Release/net10.0/linux-x64/publish";
        var workerPath = Path.Combine(workerDir, "DistributedExecutionEngine.WorkerHost");

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = workerPath,
                WorkingDirectory = workerDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            }
        };
        
        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data == null) return;
            
            logger.LogInformation("[Worker] {Output}", e.Data);
            Console.WriteLine($"[Worker] {e.Data}");
        };
        
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data == null) return;
            
            logger.LogError("[Worker] {Output}", e.Data);
            Console.WriteLine($"[Worker] {e.Data}");
        };
        
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        
        logger.LogCritical("Worker started. PID {Pid}", process.Id);
        
        return Task.CompletedTask;
    }

    public Task StopWorkerAsync(int processId, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}