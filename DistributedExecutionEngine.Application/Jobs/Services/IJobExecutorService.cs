namespace DistributedExecutionEngine.Application.Jobs.Services;

public interface IJobExecutorService
{ 
    Task<JobResult> ExecuteJob<TJob>(TJob job);
}