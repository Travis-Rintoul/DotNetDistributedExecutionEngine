using DistributedExecutionEngine.Application;
using DistributedExecutionEngine.Application.Jobs.Services;
using DistributedExecutionEngine.Domain.Entities;
using DistributedExecutionEngine.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.MapPost("/jobs", async (IJobService jobService) =>
{
    await jobService.ScheduleJob(Job.Create("GENERIC-JOB", null));
});

app.Run();