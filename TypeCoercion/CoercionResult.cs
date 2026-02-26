using System;

namespace TypeCoercion;

/// <summary>
/// Result of a type coercion operation. Use <see cref="Ok"/> and <see cref="Fail(string, CoercionErrorCode)"/>
/// factory methods to construct instances.
/// </summary>
public readonly struct CoercionResult : IEquatable<CoercionResult>
{
    /// <summary>Whether the coercion succeeded.</summary>
    public bool Success { get; }

    /// <summary>The coerced value when <see cref="Success"/> is <c>true</c>; otherwise <c>null</c>.</summary>
    public object? Value { get; }

    /// <summary>Error message when <see cref="Success"/> is <c>false</c>; otherwise <see cref="string.Empty"/>.</summary>
    public string Error { get; }

    /// <summary>Categorized error code when <see cref="Success"/> is <c>false</c>; otherwise <see cref="CoercionErrorCode.None"/>.</summary>
    public CoercionErrorCode ErrorCode { get; }

    /// <summary>The original exception that caused the failure, if any.</summary>
    public Exception? OriginalException { get; }

    private CoercionResult(bool success, object? value, string error, CoercionErrorCode errorCode, Exception? originalException)
    {
        Success = success;
        Value = value;
        Error = error;
        ErrorCode = errorCode;
        OriginalException = originalException;
    }

    /// <summary>Creates a successful result containing <paramref name="value"/>.</summary>
    /// <param name="value">The coerced value.</param>
    /// <returns>A successful <see cref="CoercionResult"/>.</returns>
    public static CoercionResult Ok(object? value) => new(true, value, string.Empty, CoercionErrorCode.None, null);

    /// <summary>Creates a failure result with the specified error details.</summary>
    /// <param name="error">Human-readable error message.</param>
    /// <param name="errorCode">Categorized error code.</param>
    /// <returns>A failed <see cref="CoercionResult"/>.</returns>
    public static CoercionResult Fail(string error, CoercionErrorCode errorCode) => new(false, null, error, errorCode, null);

    /// <summary>Creates a failure result with the specified error details and the exception that caused it.</summary>
    /// <param name="error">Human-readable error message.</param>
    /// <param name="errorCode">Categorized error code.</param>
    /// <param name="originalException">The exception that caused the failure.</param>
    /// <returns>A failed <see cref="CoercionResult"/>.</returns>
    public static CoercionResult Fail(string error, CoercionErrorCode errorCode, Exception originalException)
        => new(false, null, error, errorCode, originalException);

    /// <inheritdoc />
    public bool Equals(CoercionResult other)
        => Success == other.Success
           && Equals(Value, other.Value)
           && Error == other.Error
           && ErrorCode == other.ErrorCode;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CoercionResult other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Success, Value, Error, ErrorCode);

    /// <summary>Equality operator.</summary>
    public static bool operator ==(CoercionResult left, CoercionResult right) => left.Equals(right);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(CoercionResult left, CoercionResult right) => !left.Equals(right);
}

/// <summary>
/// Result of a strongly-typed type coercion operation. Use <see cref="Ok"/> and <see cref="Fail(string, CoercionErrorCode, Exception?)"/>
/// factory methods to construct instances.
/// </summary>
/// <typeparam name="T">The type of the expected coerced value.</typeparam>
public readonly struct CoercionResult<T> : IEquatable<CoercionResult<T>>
{
    /// <summary>Whether the coercion succeeded.</summary>
    public bool Success { get; }

    /// <summary>The coerced value when <see cref="Success"/> is <c>true</c>; otherwise <c>default(T)</c>.</summary>
    public T? Value { get; }

    /// <summary>Error message when <see cref="Success"/> is <c>false</c>; otherwise <see cref="string.Empty"/>.</summary>
    public string Error { get; }

    /// <summary>Categorized error code when <see cref="Success"/> is <c>false</c>; otherwise <see cref="CoercionErrorCode.None"/>.</summary>
    public CoercionErrorCode ErrorCode { get; }

    /// <summary>The original exception that caused the failure, if any.</summary>
    public Exception? OriginalException { get; }

    private CoercionResult(bool success, T? value, string error, CoercionErrorCode errorCode, Exception? originalException)
    {
        Success = success;
        Value = value;
        Error = error;
        ErrorCode = errorCode;
        OriginalException = originalException;
    }

    /// <summary>Creates a successful result containing <paramref name="value"/>.</summary>
    /// <param name="value">The coerced value.</param>
    /// <returns>A successful <see cref="CoercionResult{T}"/>.</returns>
    public static CoercionResult<T> Ok(T? value) => new(true, value, string.Empty, CoercionErrorCode.None, null);

    /// <summary>Creates a failure result with the specified error details.</summary>
    /// <param name="error">Human-readable error message.</param>
    /// <param name="errorCode">Categorized error code.</param>
    /// <param name="originalException">An optional inner exception.</param>
    /// <returns>A failed <see cref="CoercionResult{T}"/>.</returns>
    public static CoercionResult<T> Fail(string error, CoercionErrorCode errorCode, Exception? originalException = null) 
        => new(false, default, error, errorCode, originalException);
        
    /// <summary>Implicit conversion to non-generic <see cref="CoercionResult"/>.</summary>
    public static implicit operator CoercionResult(CoercionResult<T> result)
    {
        if (result.Success) return CoercionResult.Ok(result.Value);
        return result.OriginalException is not null
            ? CoercionResult.Fail(result.Error, result.ErrorCode, result.OriginalException)
            : CoercionResult.Fail(result.Error, result.ErrorCode);
    }
    
    /// <inheritdoc />
    public bool Equals(CoercionResult<T> other)
        => Success == other.Success
           && System.Collections.Generic.EqualityComparer<T?>.Default.Equals(Value, other.Value)
           && Error == other.Error
           && ErrorCode == other.ErrorCode;

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CoercionResult<T> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Success, Value, Error, ErrorCode);

    /// <summary>Equality operator.</summary>
    public static bool operator ==(CoercionResult<T> left, CoercionResult<T> right) => left.Equals(right);

    /// <summary>Inequality operator.</summary>
    public static bool operator !=(CoercionResult<T> left, CoercionResult<T> right) => !left.Equals(right);
}
