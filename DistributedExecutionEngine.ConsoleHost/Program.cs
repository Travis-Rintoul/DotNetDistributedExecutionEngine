using System.Diagnostics;
using DistributedExecutionEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<OrchestratorDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default"));
});

var host = builder.Build();
var workerPath = "../../../../DistributedExecutionEngine.WorkerHost/bin/Debug/net10.0/DistributedExecutionEngine.WorkerHost";
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
        Console.WriteLine($"[Worker] {e.Data}");
};

process.Start();
process.BeginOutputReadLine();

await host.RunAsync();