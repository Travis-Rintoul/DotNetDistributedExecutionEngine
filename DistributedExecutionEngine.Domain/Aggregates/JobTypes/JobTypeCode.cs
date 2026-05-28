using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Domain.Aggregates.JobTypes;

public readonly record struct JobTypeCode
{
    public string Value { get; }

    private JobTypeCode(string value)
    {
        Value = value;
    }

    public static Result<JobTypeCode, string> From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<JobTypeCode, string>.Failure("InvalidJobType");

        return Result<JobTypeCode, string>.Success(new JobTypeCode(value.Trim()));
    }

    public override string ToString() => Value;
}