namespace DistributedExecutionEngine.Infrastructure.Persistence.Records;

public class JobTypeRecord
{
    public long Id { get; set; }
    public string Key { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public int DefaultMaxAttempts { get; set; }
    public int? DefaultTimeoutSeconds { get; set; }
}