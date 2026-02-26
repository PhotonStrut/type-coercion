using System;

namespace QueryBuilder.Core.Coercion;

/// <summary>
/// Thrown when <see cref="TypeCoercion.Coerce(object?, Type)"/> cannot convert a value to the target type.
/// </summary>
public sealed class TypeCoercionException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance with the specified error message and error code.
    /// </summary>
    /// <param name="message">The error message describing the coercion failure.</param>
    /// <param name="errorCode">The categorized error code.</param>
    public TypeCoercionException(string message, CoercionErrorCode errorCode)
        : base(message) => ErrorCode = errorCode;

    /// <summary>
    /// Initializes a new instance with the specified error message, error code, and inner exception.
    /// </summary>
    /// <param name="message">The error message describing the coercion failure.</param>
    /// <param name="errorCode">The categorized error code.</param>
    /// <param name="innerException">The exception that caused this coercion failure.</param>
    public TypeCoercionException(string message, CoercionErrorCode errorCode, Exception innerException)
        : base(message, innerException) => ErrorCode = errorCode;

    /// <summary>The categorized error code for this coercion failure.</summary>
    public CoercionErrorCode ErrorCode { get; }
}
