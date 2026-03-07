namespace DistributedExecutionEngine.Application.Jobs.Services;

public class JobExecutorService :  IJobExecutorService
{
    public Task<JobResult> ExecuteJob<TJob>(TJob job)
    {
        throw new NotImplementedException();
    }
}