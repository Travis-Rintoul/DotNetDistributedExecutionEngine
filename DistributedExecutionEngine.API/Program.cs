using DistributedExecutionEngine.Application;
using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Application.Features.Jobs.Queries;
using DistributedExecutionEngine.Application.Features.Jobs.Scheduling;
using DistributedExecutionEngine.Application.Features.JobTypes.Queries;
using DistributedExecutionEngine.Application.Features.Workers.Queries;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Common;
using DistributedExecutionEngine.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.MapPost("/jobs", async (ICommandDispatcher dispatcher) =>
{
    return (await dispatcher.SendAsync(new ScheduleJobCommand(Job.Create("GENERIC-JOB", null))))
        .Match(
            success => Results.Ok(success.Value),
            Results.BadRequest
        );
});

app.MapGet("/jobs", async (IQueryDispatcher dispatcher) =>
    Results.Ok(await dispatcher.SendAsync(new GetJobsQuery())));

app.MapGet("/workers", async (IQueryDispatcher dispatcher) =>
    Results.Ok(await dispatcher.SendAsync(new GetWorkersQuery())));

app.MapGet("/job-types", async (IQueryDispatcher dispatcher) =>
    Results.Ok(await dispatcher.SendAsync(new GetJobTypesQuery())));

app.Run();