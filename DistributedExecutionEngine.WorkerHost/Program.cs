using DistributedExecutionEngine.Application;
using DistributedExecutionEngine.Infrastructure;
using DistributedExecutionEngine.WorkerHost;
using Microsoft.Extensions.Hosting;
using DependencyInjection = DistributedExecutionEngine.WorkerHost.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddWorkerHost();
var host = builder.Build();
await host.RunAsync();