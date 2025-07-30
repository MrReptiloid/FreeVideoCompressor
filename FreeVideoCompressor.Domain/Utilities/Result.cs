using System.Diagnostics.CodeAnalysis;

namespace FreeVideoCompressor.Domain.Utilities;

public readonly struct Result<TResult, TFailure> : IEquatable<Result<TResult, TFailure>>
{
    private readonly TResult? _value;
    private readonly TFailure? _error;

    private Result(TResult? value, bool isOk)
    {
        _value = value;
        _error = default;
        IsOk = isOk;
    }

    private Result(TFailure error)
    {
        _value = default;
        _error = error;
        IsOk = false;
    }
    
    public static Result<TResult, TFailure> Ok(TResult value)
    {
        return new Result<TResult, TFailure>(value, true);
    }

    public static Result<TResult, TFailure> Err(TFailure error)
    {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        return new Result<TResult, TFailure>(error);
    }
    
    [MemberNotNullWhen(true, nameof(_value))]
    [MemberNotNullWhen(false, nameof(_error))]
    public bool IsOk { get; }

    [MemberNotNullWhen(false, nameof(_value))]
    [MemberNotNullWhen(true, nameof(_error))]
    public bool IsErr => !IsOk;

    public TResult Unwrap()
    {
        return IsOk ? _value! : throw new InvalidOperationException("Cannot unwrap an Err result");
    }

    public TFailure UnwrapErr()
    {
        return IsOk ? throw new InvalidOperationException("Cannot unwrap an Ok result") : _error!;
    }
    
    public Result<TNext, TFailure> Map<TNext>(Func<TResult, TNext> mapFn)
    {
        return IsOk ? Result<TNext, TFailure>.Ok(mapFn(_value!)) : Result<TNext, TFailure>.Err(_error!);
    }

    public Result<TResult, TENext> MapErr<TENext>(Func<TFailure, TENext> mapFn) where TENext : notnull
    {
        return IsOk ? Result<TResult, TENext>.Ok(_value!) : Result<TResult, TENext>.Err(mapFn(_error!));
    }

    public Result<TNext, TFailure> Bind<TNext>(Func<TResult, Result<TNext, TFailure>> bindFn)
    {
        return IsOk ? bindFn(_value!) : Result<TNext, TFailure>.Err(_error!);
    }
    
    public TOut Match<TOut>(Func<TResult, TOut> ok, Func<TFailure, TOut> err)
    {
        return IsOk ? ok(_value!) : err(_error!);
    }
    
    public override string ToString()
    {
        return IsOk ? "Ok" : "Err";
    }

    public bool Equals(Result<TResult, TFailure> other)
    {
        return IsOk == other.IsOk &&
               (IsOk
                   ? EqualityComparer<TResult?>.Default.Equals(_value, other._value)
                   : EqualityComparer<TFailure>.Default.Equals(_error, other._error));
    }

    public override bool Equals(object? obj)
    {
        return obj is Result<TResult, TFailure> other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + IsOk.GetHashCode();
            hash = hash * 31 + (IsOk
                ? EqualityComparer<TResult?>.Default.GetHashCode(_value)
                : EqualityComparer<TFailure>.Default.GetHashCode(_error!));
            return hash;
        }
    }

    public static bool operator ==(Result<TResult, TFailure> left, Result<TResult, TFailure> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Result<TResult, TFailure> left, Result<TResult, TFailure> right)
    {
        return !left.Equals(right);
    }
}