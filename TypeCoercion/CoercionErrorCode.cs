namespace QueryBuilder.Core.Coercion;

/// <summary>
/// Error codes indicating why a type coercion operation failed.
/// </summary>
public enum CoercionErrorCode
{
    /// <summary>No error. Used on successful <see cref="CoercionResult"/> instances.</summary>
    None = 0,

    /// <summary>The source value's type is not supported for the target type.</summary>
    UnsupportedSourceType,

    /// <summary>The source value's format is invalid for the target type (e.g., non-numeric string to int).</summary>
    InvalidFormat,

    /// <summary>The source value exceeds the range of the target type.</summary>
    Overflow,

    /// <summary>General conversion failure that does not fit a more specific category.</summary>
    ConversionFailed,

    /// <summary>The value is not a valid member of the target enum type.</summary>
    InvalidEnumMember
}
