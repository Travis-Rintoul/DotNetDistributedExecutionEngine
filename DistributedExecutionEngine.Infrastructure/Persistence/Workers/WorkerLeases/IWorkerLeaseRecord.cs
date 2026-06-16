using DistributedExecutionEngine.Domain.Aggregates.Workers;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerLeases;

public interface IWorkerLeaseRecord
{
    public WorkerLeaseStatusCode LeaseStatusCode { get; set; }
    public int? SupervisionLeasedBy { get; set; }
    public DateTimeOffset? SupervisionLeasedUtc { get; set; }
    public DateTimeOffset? SupervisionLeaseExpiresUtc { get; set; }
}