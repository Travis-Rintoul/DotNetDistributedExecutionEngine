using System.Linq.Expressions;
using System.Reflection;

namespace DistributedExecutionEngine.Infrastructure.Persistence.Mapping;

public sealed record PersistenceShapeField<TRecord, TValue>(
    string Name,
    Func<TRecord, TValue> Get,
    Action<TRecord, TValue> Set,
    TValue ResetValue)
    : IPersistenceShapeField<TRecord>
{
    public bool IsDefault(TRecord record)
        => EqualityComparer<TValue>.Default.Equals(Get(record), ResetValue);

    public void Reset(TRecord record)
        => Set(record, ResetValue);
}

public static class PersistenceFields
{
    public static PersistenceShapeField<TRecord, TValue> Of<TRecord, TValue>(
        Expression<Func<TRecord, TValue>> property)
        => Of(property, resetValue: default!);
    
    public static PersistenceShapeField<TRecord, TValue> Of<TRecord, TValue>(Expression<Func<TRecord, TValue>> property, TValue resetValue)
    {
        var propertyInfo = GetPropertyInfo(property);

        return new PersistenceShapeField<TRecord, TValue>(
            Name: propertyInfo.Name,
            Get: property.Compile(),
            Set: CreateSetter<TRecord, TValue>(propertyInfo),
            ResetValue: resetValue);
    }

    private static PropertyInfo GetPropertyInfo<TRecord, TValue>(Expression<Func<TRecord, TValue>> property)
    {
        if (property.Body is MemberExpression { Member: PropertyInfo propertyInfo })
            return propertyInfo;

        throw new ArgumentException(
            "Expression must be a direct property access, e.g. x => x.ProcessId.",
            nameof(property));
    }

    private static Action<TRecord, TValue> CreateSetter<TRecord, TValue>(PropertyInfo propertyInfo)
    {
        if (propertyInfo.SetMethod is null)
            throw new ArgumentException(
                $"Property '{propertyInfo.Name}' does not have a setter.");

        var record = Expression.Parameter(typeof(TRecord), "record");
        var value = Expression.Parameter(typeof(TValue), "value");

        var assignment = Expression.Assign(
            Expression.Property(record, propertyInfo),
            value);

        return Expression
            .Lambda<Action<TRecord, TValue>>(assignment, record, value)
            .Compile();
    }
}