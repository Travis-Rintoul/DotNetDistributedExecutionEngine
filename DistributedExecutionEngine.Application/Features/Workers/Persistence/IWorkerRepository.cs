using DistributedExecutionEngine.Application.Abstractions.Persistence;
using DistributedExecutionEngine.Domain.Aggregates.Workers;

namespace DistributedExecutionEngine.Application.Features.Workers.Persistence;

public interface IWorkerRepository : IAggregateRepository<Worker,  WorkerId> { }