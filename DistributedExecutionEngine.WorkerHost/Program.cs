using DistributedExecutionEngine.Application;
using DistributedExecutionEngine.Infrastructure;
using DistributedExecutionEngine.WorkerHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<WorkerOptions>(o =>
{
    o.WorkerId = builder.Configuration.GetValue<int>("worker-id");
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWorkerHost();
var host = builder.Build();
await host.RunAsync();