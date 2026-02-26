using System;

namespace QueryBuilder.Core.Coercion;

public readonly struct CoercionResult<T> : IEquatable<CoercionResult<T>>
{
    public bool Success { get; }
    public T? Value { get; }
    public string Error { get; }
    public CoercionErrorCode ErrorCode { get; }
    public Exception? OriginalException { get; }

    private CoercionResult(bool success, T? value, string error, CoercionErrorCode errorCode, Exception? originalException)
    {
        Success = success;
        Value = value;
        Error = error;
        ErrorCode = errorCode;
        OriginalException = originalException;
    }

    public static CoercionResult<T> Ok(T? value) => new(true, value, string.Empty, CoercionErrorCode.None, null);

    public static CoercionResult<T> Fail(string error, CoercionErrorCode errorCode, Exception? originalException = null) 
        => new(false, default, error, errorCode, originalException);
        
    public static implicit operator CoercionResult(CoercionResult<T> result)
    {
        if (result.Success) return CoercionResult.Ok(result.Value);
        return CoercionResult.Fail(result.Error, result.ErrorCode, result.OriginalException!);
    }
    
    public bool Equals(CoercionResult<T> other)
        => Success == other.Success
           && System.Collections.Generic.EqualityComparer<T?>.Default.Equals(Value, other.Value)
           && Error == other.Error
           && ErrorCode == other.ErrorCode;

    public override bool Equals(object? obj) => obj is CoercionResult<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Success, Value, Error, ErrorCode);

    public static bool operator ==(CoercionResult<T> left, CoercionResult<T> right) => left.Equals(right);

    public static bool operator !=(CoercionResult<T> left, CoercionResult<T> right) => !left.Equals(right);
}
