using System.Diagnostics;
using System.Linq.Expressions;
using DistributedExecutionEngine.Domain.Common;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Infrastructure.Persistence.Mapping;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers.Runtime;

public interface IWorkerRuntimeMapper : IPersistenceShapeMapper<IWorkerRuntimeRecord, WorkerStatusCode, WorkerRuntime>, IPersistenceShapeWriter<IWorkerRuntimeRecord, WorkerRuntime> { }

internal class WorkerRuntimeMapper : IWorkerRuntimeMapper
{
    private readonly PersistenceShapeEnforcer<IWorkerRuntimeRecord> _shapeEnforcer = new(All);

    private static PersistenceShapeField<IWorkerRuntimeRecord, TValue> Field<TValue>(
        Expression<Func<IWorkerRuntimeRecord, TValue>> selector)
        => PersistenceFields.Of(selector);
    
    private static PersistenceShapeField<IWorkerRuntimeRecord, TValue> Field<TValue>(
        Expression<Func<IWorkerRuntimeRecord, TValue>> selector, TValue defaultValue)
        => PersistenceFields.Of(selector, defaultValue);
    
    private static PersistenceShape<IWorkerRuntimeRecord> Shape(
        IEnumerable<IPersistenceShapeField<IWorkerRuntimeRecord>> required,
        IEnumerable<IPersistenceShapeField<IWorkerRuntimeRecord>> optional)
        => PersistenceShape<IWorkerRuntimeRecord>.For(required, optional, All);
    

    private static readonly PersistenceShapeField<IWorkerRuntimeRecord, int?> ProcessIdField
        = Field(x => x.ProcessId);

    private static readonly PersistenceShapeField<IWorkerRuntimeRecord, string?> Hostname        
        = Field(x => x.Hostname);
    
    private static readonly PersistenceShapeField<IWorkerRuntimeRecord, string?> MachineName        
        = Field(x => x.MachineName);

    private static readonly PersistenceShapeField<IWorkerRuntimeRecord, DateTimeOffset?> ProcessStartedUtc        
        = Field(x => x.ProcessStartedUtc);
    
    private static readonly PersistenceShapeField<IWorkerRuntimeRecord, DateTimeOffset?> RunningSinceUtc        
        = Field(x => x.RunningSinceUtc);

    private static readonly PersistenceShapeField<IWorkerRuntimeRecord, DateTimeOffset?> LastHeartbeatUtc        
        = Field(x => x.LastHeartbeatUtc);
    
    private static readonly PersistenceShapeField<IWorkerRuntimeRecord, int> StartupAttemptCount        
        = Field(x => x.StartupAttemptCount, defaultValue: 0);
    
    private static readonly PersistenceShapeField<IWorkerRuntimeRecord, int> MaxStartupAttemptCount        
        = Field(x => x.MaxStartupAttemptCount, defaultValue: 0);

    private static readonly IReadOnlyList<IPersistenceShapeField<IWorkerRuntimeRecord>> All =
    [
        ProcessIdField,
        Hostname,
        MachineName,
        ProcessStartedUtc,
        RunningSinceUtc,
        LastHeartbeatUtc,
        StartupAttemptCount,
        MaxStartupAttemptCount
    ];
    
    private static readonly IReadOnlyDictionary<WorkerStatusCode, PersistenceShape<IWorkerRuntimeRecord>> ByState =
        new Dictionary<WorkerStatusCode, PersistenceShape<IWorkerRuntimeRecord>>
        {
            [WorkerStatusCode.Pending] = Shape(
                required: [],
                optional: []),
            [WorkerStatusCode.Starting] = Shape(
                required: [ProcessIdField, Hostname, MachineName, ProcessStartedUtc, StartupAttemptCount, MaxStartupAttemptCount],
                optional: []),
            [WorkerStatusCode.Running] = Shape(
                required: [ProcessIdField, Hostname, MachineName, ProcessStartedUtc, RunningSinceUtc, LastHeartbeatUtc, StartupAttemptCount, MaxStartupAttemptCount],
                optional: []),
            [WorkerStatusCode.Failed] = Shape(
                required: [StartupAttemptCount, MaxStartupAttemptCount, LastHeartbeatUtc],
                optional: []),
            [WorkerStatusCode.Canceled] = Shape(
                required: [StartupAttemptCount, MaxStartupAttemptCount],
                optional: []),
            [WorkerStatusCode.Lost] = Shape(
                required: [StartupAttemptCount, MaxStartupAttemptCount],
                optional: []),
        };

    public WorkerStatusCode ToCode(WorkerRuntime record)
    {
        return record switch
        {
            WorkerRuntime.Pending => WorkerStatusCode.Pending,
            WorkerRuntime.Starting => WorkerStatusCode.Starting,
            WorkerRuntime.Running => WorkerStatusCode.Running,
            WorkerRuntime.Failed => WorkerStatusCode.Failed,
            WorkerRuntime.Canceled => WorkerStatusCode.Canceled,
            WorkerRuntime.Lost => WorkerStatusCode.Lost,
            _ => throw new UnreachableException()
        };
    }

    public Result<WorkerRuntime, string> ToDomain(IWorkerRuntimeRecord record)
    {
        if (!ByState.TryGetValue(record.StatusCode, out var shape))
            return Result<WorkerRuntime, string>.Failure(
                $"Unknown worker runtime status code '{record.StatusCode}'.");
        
        var validation = _shapeEnforcer.Validate(record, shape);
        if (validation.IsFailure)
            return Result<WorkerRuntime, string>.Failure(validation.Error);

        return record.StatusCode switch
        {
            WorkerStatusCode.Pending => Result<WorkerRuntime, string>.Success(new WorkerRuntime.Pending()),
            WorkerStatusCode.Starting => ToStarting(record),
            WorkerStatusCode.Running => ToRunning(record),
            WorkerStatusCode.Failed => ToFailed(record),
            WorkerStatusCode.Canceled => ToCanceled(record),
            WorkerStatusCode.Lost => ToLost(record),
            _ => Result<WorkerRuntime, string>.Failure($"Unknown worker status code '{record.StatusCode}'.")
        };
    }

    public Result<Unit, string> ApplyToRecord(WorkerRuntime runtime, IWorkerRuntimeRecord record)
    {
        record.StatusCode = ToCode(runtime);

        switch (runtime)
        {
            case WorkerRuntime.Pending pending:
                break;

            case WorkerRuntime.Starting starting:
                record.ProcessId = starting.ProcessId.Value;
                record.Hostname = starting.Hostname;
                record.MachineName = starting.MachineName;
                record.ProcessStartedUtc = starting.ProcessStartedUtc.Value;
                record.StartupAttemptCount = starting.StartupAttemptCount;
                record.MaxStartupAttemptCount = starting.MaxStartupAttemptCount;
                break;

            case WorkerRuntime.Running running:
                record.ProcessId = running.ProcessId.Value;
                record.Hostname = running.Hostname;
                record.MachineName = running.MachineName;
                record.ProcessStartedUtc = running.ProcessStartedUtc.Value;
                record.RunningSinceUtc = running.RunningSinceUtc.Value;
                record.LastHeartbeatUtc = running.LastHeartbeatUtc.Value;
                record.StartupAttemptCount = running.StartupAttemptCount;
                record.MaxStartupAttemptCount = running.MaxStartupAttemptCount;
                break;

            case WorkerRuntime.Failed failed:
                record.StartupAttemptCount = failed.StartupAttemptCount;
                record.MaxStartupAttemptCount = failed.MaxStartupAttemptCount;
                record.LastHeartbeatUtc = failed.LastHeartbeatUtc.Value;
                break;

            case WorkerRuntime.Canceled canceled:
                record.StartupAttemptCount = canceled.StartupAttemptCount;
                record.MaxStartupAttemptCount = canceled.MaxStartupAttemptCount;
                break;

            case WorkerRuntime.Lost lost:
                record.StartupAttemptCount = lost.StartupAttemptCount;
                record.MaxStartupAttemptCount = lost.MaxStartupAttemptCount;
                break;
        }

        return _shapeEnforcer.Apply(record, ByState[record.StatusCode]);
    }

    private static Result<WorkerRuntime, string> ToStarting(IWorkerRuntimeRecord runtime)
        => Result<WorkerRuntime, string>.Success(
            new WorkerRuntime.Starting(
                ProcessId: ProcessId.From(runtime.ProcessId!.Value),
                Hostname: runtime.Hostname!,
                MachineName: runtime.MachineName!,
                ProcessStartedUtc: Option.FromNullable(runtime.ProcessStartedUtc),
                LastHeartbeatUtc: Option.FromNullable(runtime.LastHeartbeatUtc),
                StartupAttemptCount: runtime.StartupAttemptCount,
                MaxStartupAttemptCount: runtime.MaxStartupAttemptCount));
    
    private static Result<WorkerRuntime, string> ToRunning(IWorkerRuntimeRecord runtime)
        => Result<WorkerRuntime, string>.Success(
            new WorkerRuntime.Running(
                ProcessId: ProcessId.From(runtime.ProcessId!.Value),
                Hostname: runtime.Hostname!,
                MachineName: runtime.MachineName!,
                ProcessStartedUtc: Option.FromNullable(runtime.ProcessStartedUtc),
                RunningSinceUtc: Option.FromNullable(runtime.RunningSinceUtc),
                LastHeartbeatUtc: Option.FromNullable(runtime.LastHeartbeatUtc),
                StartupAttemptCount: runtime.StartupAttemptCount,
                MaxStartupAttemptCount: runtime.MaxStartupAttemptCount));
    
    private static Result<WorkerRuntime, string> ToFailed(IWorkerRuntimeRecord runtime)
        => Result<WorkerRuntime, string>.Success(
            new WorkerRuntime.Failed(
                StartupAttemptCount: runtime.StartupAttemptCount,
                MaxStartupAttemptCount: runtime.MaxStartupAttemptCount,
                LastHeartbeatUtc: runtime.LastHeartbeatUtc));
    
    private static Result<WorkerRuntime, string> ToCanceled(IWorkerRuntimeRecord runtime)
        => Result<WorkerRuntime, string>.Success(
            new WorkerRuntime.Canceled(
                StartupAttemptCount: runtime.StartupAttemptCount,
                MaxStartupAttemptCount: runtime.MaxStartupAttemptCount));
    
    private static Result<WorkerRuntime, string> ToLost(IWorkerRuntimeRecord runtime)
        => Result<WorkerRuntime, string>.Success(
            new WorkerRuntime.Lost(
                ProcessId: ProcessId.From(runtime.ProcessId!.Value),
                Hostname: runtime.Hostname!,
                MachineName: runtime.MachineName!,
                ProcessStartedUtc: Option.FromNullable(runtime.ProcessStartedUtc),
                RunningSinceUtc: Option.FromNullable(runtime.RunningSinceUtc),
                LastHeartbeatUtc: Option.FromNullable(runtime.LastHeartbeatUtc),
                StartupAttemptCount: runtime.StartupAttemptCount,
                MaxStartupAttemptCount: runtime.MaxStartupAttemptCount));
    
}