using DistributedExecutionEngine.Application.Abstractions.Messaging;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;
using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Application.Features.Jobs.Scheduling;

public class ScheduleJobCommand(Job job) : ICommand<Result<JobId, ScheduleJobError>>;