namespace DistributedExecutionEngine.Domain.Common;

public class Option<T>
{
    private T _value;
    private readonly bool _hasValue;

    private Option()
    {
        _value = default!;
        _hasValue = false;
    }

    private Option(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        _value = value;
        _hasValue = true;
    }

    public bool IsSome => _hasValue;
    public bool IsNone => !_hasValue;

    public static Option<T> Some(T value) => new(value);
    public static Option<T> None => new();

    public T Value =>
        _hasValue
            ? _value
            : throw new InvalidOperationException("Option has no value.");

    public Option<TResult> Map<TResult>(Func<T, TResult> map)
    {
        ArgumentNullException.ThrowIfNull(map);

        return _hasValue
            ? Option<TResult>.Some(map(_value))
            : Option<TResult>.None;
    }

    public T ValueOr(T defaultValue) =>
        _hasValue ? _value : defaultValue;
    
    public T ValueOrThrow()
    {
        if (IsNone)
            throw new InvalidOperationException("Cannot access the value of None.");

        return _value!;
    }

    public T ValueOrThrow(Func<Exception> exceptionFactory)
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        return _hasValue
            ? _value
            : throw exceptionFactory();
    }
}

public static class OptionTaskExtensions
{
    public static async Task<T> ValueOrThrowAsync<T>(this Task<Option<T>> optionTask, Func<Exception> exceptionFactory)
    {
        ArgumentNullException.ThrowIfNull(optionTask);
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        var option = await optionTask.ConfigureAwait(false);

        return option.ValueOrThrow(exceptionFactory);
    }
}