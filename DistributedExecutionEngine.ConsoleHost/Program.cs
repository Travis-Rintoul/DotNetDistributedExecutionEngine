using DistributedExecutionEngine.Application;
using DistributedExecutionEngine.ConsoleHost;
using DistributedExecutionEngine.Infrastructure;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWorkerProvisionerHost();
builder.Services.AddSupervisorHost();
builder.Services.AddApplicationServices();

var host = builder.Build();
await host.RunAsync();