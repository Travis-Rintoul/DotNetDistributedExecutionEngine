using DistributedExecutionEngine.Application;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Infrastructure;
using DistributedExecutionEngine.WorkerHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<WorkerOptions>(o =>
{
    o.WorkerId = WorkerId.From(builder.Configuration.GetValue<Guid>("worker-id"));
});

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWorkerHost();
var host = builder.Build();
await host.RunAsync();