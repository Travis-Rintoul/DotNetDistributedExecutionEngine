using DistributedExecutionEngine.Application;
using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Jobs.Scheduling;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Common;
using DistributedExecutionEngine.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.MapPost("/jobs", async (ICommandHandler<ScheduleJobCommand, Result<JobId, ScheduleJobError>> jobCommandHandler) =>
{
    await jobCommandHandler.HandleAsync(new ScheduleJobCommand(Job.Create("GENERIC-JOB", null)));
});

app.Run();