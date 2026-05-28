namespace DistributedExecutionEngine.Domain.Common;

public sealed class Result<TResult, TError>
{
    private readonly TResult? _value;
    private readonly TError? _error;
    
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public TResult Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException("Result is not successful.");

    public TError Error =>
        IsFailure
            ? _error!
            : throw new InvalidOperationException("Result is successful.");
    
    private Result(bool isSuccess, TResult? value, TError? error)
    {
        IsSuccess = isSuccess;
        _value = value;
        _error = error;
    }
    
    public static Result<TResult, TError> Success(TResult value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Result<TResult, TError>(true, value, default);
    }
    
    public static Result<TResult, TError> Failure(TError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new Result<TResult, TError>(false, default, error);
    }

    public TOut Match<TOut>(Func<TResult, TOut> success, Func<TError, TOut> failure) =>
        IsSuccess ? success(_value!) : failure(_error!);
    
    public TResult ValueOr(TResult fallback) =>
        IsSuccess ? _value! : fallback;
    
    public TResult ValueOrThrow(Func<TError, Exception> exceptionFactory) =>
        IsSuccess
            ? _value!
            : throw exceptionFactory(_error!);

    public Option<TResult> ToOption() =>
        IsSuccess
            ? Option<TResult>.Some(_value!)
            : Option<TResult>.None;

}