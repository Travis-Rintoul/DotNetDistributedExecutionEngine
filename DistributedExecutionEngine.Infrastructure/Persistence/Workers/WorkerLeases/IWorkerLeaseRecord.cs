namespace DistributedExecutionEngine.Infrastructure.Persistence.Workers.WorkerLeases;

public interface IWorkerLeaseRecord
{
    public WorkerLeaseStatusCode LeaseStatusCode { get; set; }
    public int? SupervisorId { get; set; }
    public DateTimeOffset? ClaimedUtc { get; set; }
}