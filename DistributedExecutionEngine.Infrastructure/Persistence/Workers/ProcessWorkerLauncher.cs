using System.Diagnostics;
using DistributedExecutionEngine.Application.Abstractions;
using DistributedExecutionEngine.Application.Features.Workers.Supervision;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers;

public sealed class ProcessWorkerLauncher(
    ILogger<ProcessWorkerLauncher> logger,
    IConfiguration configuration
) : IWorkerProcessLauncher
{
    public Task<Result<ProcessId, string>> LaunchAsync(WorkerId workerId, CancellationToken token)
    {
        var workerDir = "/home/travis/Projects/DotNetDistributedExecutionEngine/DistributedExecutionEngine.WorkerHost/bin/Release/net10.0/linux-x64/publish";
        var workerPath = Path.Combine(workerDir, "DistributedExecutionEngine.WorkerHost");

        var processInfo = new ProcessStartInfo()
        {
            FileName = workerPath,
            WorkingDirectory = workerDir,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        
        processInfo.ArgumentList.Add("--worker-id");
        processInfo.ArgumentList.Add(workerId.ToString());
        
        var process = new Process
        {
            StartInfo = processInfo
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

        return Task.FromResult(Result.Success<ProcessId, string>(new ProcessId(process.Id)));
    }

    public Task StopWorkerAsync(int processId, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}