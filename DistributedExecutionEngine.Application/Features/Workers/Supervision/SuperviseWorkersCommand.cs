using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Supervision;

public sealed record SuperviseWorkersCommand(SupervisorId SupervisorId) : ICommand<Result<Unit, string>>;