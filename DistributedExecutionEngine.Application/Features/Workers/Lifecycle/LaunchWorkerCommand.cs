using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Aggregates.Supervisor;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Lifecycle;

public record LaunchWorkerCommand(WorkerId WorkerId, SupervisorId SupervisorId) : ICommand<Result<ProcessId, string>> { }