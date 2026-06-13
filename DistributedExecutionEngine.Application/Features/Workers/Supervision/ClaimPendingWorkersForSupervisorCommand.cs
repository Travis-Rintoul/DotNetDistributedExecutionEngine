using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Aggregates.Supervisor;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Supervision;

public record ClaimPendingWorkersForSupervisorCommand(SupervisorId SupervisorId) : ICommand<Result<IReadOnlyList<WorkerId>, string>> { }