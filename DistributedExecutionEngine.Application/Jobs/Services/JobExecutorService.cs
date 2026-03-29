using System;
using System.Threading.Tasks;

namespace DistributedExecutionEngine.Application.Jobs.Services;

public class JobExecutorService :  IJobExecutorService
{
    public Task<JobResult> ExecuteJob<TJob>(TJob job)
    {
        return Task.FromResult(new JobResult() { Message = "Success" });
    }
}