using System;

namespace QueryBuilder.Core.Coercion.Extensions;

/// <summary>
/// Extension methods for coercing string values.
/// </summary>
public static class StringCoercionExtensions
{
    /// <summary>
    /// Coerces a string to the specified type, throwing on failure.
    /// </summary>
    public static T? CoerceTo<T>(this string? value, TypeCoercionOptions? options = null)
        => TypeCoercion.Coerce<T>(value, options ?? TypeCoercionOptions.Default);

    /// <summary>
    /// Attempts to coerce a string to the specified type without throwing.
    /// </summary>
    public static CoercionResult<T> TryCoerceTo<T>(this string? value, TypeCoercionOptions? options = null)
        => TypeCoercion.TryCoerce<T>(value, options ?? TypeCoercionOptions.Default);

    /// <summary>
    /// Attempts to coerce a string to the specified type and outputs the result.
    /// </summary>
    public static bool TryCoerceTo<T>(this string? value, out T? result, TypeCoercionOptions? options = null)
    {
        var coercionResult = TypeCoercion.TryCoerce<T>(value, options ?? TypeCoercionOptions.Default);
        result = coercionResult.Success ? coercionResult.Value : default;
        return coercionResult.Success;
    }
}
