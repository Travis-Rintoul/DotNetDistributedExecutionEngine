using DistributedExecutionEngine.Domain.Common;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Mapping;

public sealed class PersistenceShapeEnforcer<TRecord>(IReadOnlyCollection<IPersistenceShapeField<TRecord>> allFields)
{
    public Result<Unit, string> Validate(TRecord record, PersistenceShape<TRecord> shape)
    {
        foreach (var required in shape.Required)
            if (required.IsDefault(record))
                return Result<Unit, string>.Failure(
                    $"Required field '{required.Name}' was not set.");

        foreach (var forbidden in shape.Forbidden)
            if (!forbidden.IsDefault(record))
                return Result<Unit, string>.Failure(
                    $"Forbidden field '{forbidden.Name}' was set.");

        return Result.Success<string>();
    }

    public Result<Unit, string> Apply(TRecord record, PersistenceShape<TRecord> shape)
    {
        ResetForbidden(record, shape);
        return ValidateRequired(record, shape);
    }

    public void ResetForbidden(TRecord record, PersistenceShape<TRecord> shape)
    {
        foreach (var field in allFields)
            if (shape.Forbidden.Contains(field))
                field.Reset(record);
    }

    public Result<Unit, string> ValidateRequired(TRecord record, PersistenceShape<TRecord> shape)
    {
        foreach (var required in shape.Required)
            if (required.IsDefault(record))
                return Result<Unit, string>.Failure(
                    $"Required field '{required.Name}' was not set.");

        return Result.Success<string>();
    }
}