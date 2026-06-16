using System.Windows.Input;
using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Aggregates.Workers;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Workers.Reconciliation;

public sealed record EnsureWorkerCapacityCommand() : ICommand<Result<Option<WorkerId>, string>>;