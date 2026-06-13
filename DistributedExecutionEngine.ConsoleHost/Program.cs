using DistributedExecutionEngine.Application;
using DistributedExecutionEngine.ConsoleHost;
using DistributedExecutionEngine.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWorkerProvisionerHost();
builder.Services.AddSupervisorHost();
builder.Services.AddApplicationServices();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var host = builder.Build();
await host.RunAsync();