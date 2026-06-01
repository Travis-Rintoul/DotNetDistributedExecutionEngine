using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Supervision;

public sealed record ClaimPendingWorkerCommand(SupervisorId supervisorId) : ICommand<Result<Option<WorkerId>, string>>;
