using System;

namespace TypeCoercion.Extensions;

/// <summary>
/// Extension methods for coercing string values.
/// </summary>
public static class StringCoercionExtensions
{
    /// <summary>
    /// Coerces a string to the specified type, throwing on failure.
    /// </summary>
    public static T? CoerceTo<T>(this string? value, TypeCoercionOptions? options = null)
        => TypeCoercer.Coerce<T>(value, options ?? TypeCoercionOptions.Default);

    /// <summary>
    /// Attempts to coerce a string to the specified type without throwing.
    /// </summary>
    public static CoercionResult<T> TryCoerceTo<T>(this string? value, TypeCoercionOptions? options = null)
        => TypeCoercer.TryCoerce<T>(value, options ?? TypeCoercionOptions.Default);

    /// <summary>
    /// Attempts to coerce a string to the specified type and outputs the result.
    /// </summary>
    public static bool TryCoerceTo<T>(this string? value, out T? result, TypeCoercionOptions? options = null)
    {
        var coercionResult = TypeCoercer.TryCoerce<T>(value, options ?? TypeCoercionOptions.Default);
        result = coercionResult.Success ? coercionResult.Value : default;
        return coercionResult.Success;
    }

    /// <summary>
    /// Coerces a string to the specified type and returns <c>default</c> on failure.
    /// </summary>
    public static T? CoerceToOrDefault<T>(this string? value, TypeCoercionOptions? options = null)
        => TypeCoercer.CoerceOrDefault<T>(value, options ?? TypeCoercionOptions.Default);

    /// <summary>
    /// Coerces a string to the specified type and returns <c>null</c> on failure.
    /// For non-nullable value types, this method throws when coercion fails because null cannot be represented.
    /// </summary>
    public static T? CoerceToOrNull<T>(this string? value, TypeCoercionOptions? options = null)
        => TypeCoercer.CoerceOrNull<T>(value, options ?? TypeCoercionOptions.Default);

    /// <summary>
    /// Coerces a string to the specified type and returns <paramref name="fallbackValue"/> on failure.
    /// </summary>
    public static T? CoerceToOrFallback<T>(this string? value, T? fallbackValue, TypeCoercionOptions? options = null)
        => TypeCoercer.CoerceOrFallback(value, fallbackValue, options ?? TypeCoercionOptions.Default);
}
