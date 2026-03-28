using DistributedExecutionEngine.Application;
using DistributedExecutionEngine.ConsoleHost;
using DistributedExecutionEngine.Infrastructure;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddProvisionerHost();
builder.Services.AddLauncherHost();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var host = builder.Build();
await host.RunAsync();