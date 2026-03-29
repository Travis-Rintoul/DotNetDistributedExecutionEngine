using System.Threading;
using System.Threading.Tasks;
using DistributedExecutionEngine.Domain.Entities;

namespace DistributedExecutionEngine.Application.Jobs;

public interface IJobHandler<in TJob>
{
    Task<JobResult> ExecuteAsync(TJob job, CancellationToken cancellationToken);
}