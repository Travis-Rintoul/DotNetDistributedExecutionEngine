using DistributedExecutionEngine.Domain.Aggregates.Workers;

namespace DistributedExecutionEngine.Application.Abstractions.Persistence;

public interface IWorkerRepository : IAggregateRepository<Worker, WorkerId> { }