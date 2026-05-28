using DistributedExecutionEngine.Application.Abstractions.Persistence;
using DistributedExecutionEngine.Domain.Aggregates.Jobs;

namespace DistributedExecutionEngine.Application.Features.Jobs.Persistence;

public interface IJobRepository : IAggregateRepository<Job, JobId> {}