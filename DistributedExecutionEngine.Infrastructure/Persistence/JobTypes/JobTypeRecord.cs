namespace DistributedExecutionEngine.Infrastructure.Persistence.JobTypes;

public class JobTypeRecord
{
    public long Id { get; set; }
    public string Code { get; set; } = null!;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int DefaultMaxAttempts { get; set; }
    public int? DefaultTimeoutSeconds { get; set; }
}