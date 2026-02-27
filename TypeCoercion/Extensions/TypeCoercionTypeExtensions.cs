using System;

namespace TypeCoercion.Extensions;

/// <summary>
/// Extension methods for coercing values to a runtime <see cref="Type"/>.
/// </summary>
public static class TypeCoercionTypeExtensions
{
    /// <summary>
    /// Attempts to coerce <paramref name="value"/> to <paramref name="targetType"/> without throwing.
    /// </summary>
    public static CoercionResult TryCoerce(this Type targetType, object? value, TypeCoercionOptions? options = null)
        => TypeCoercer.TryCoerce(value, targetType, options ?? TypeCoercionOptions.Default);

    /// <summary>
    /// Coerces <paramref name="value"/> to <paramref name="targetType"/>, throwing on failure.
    /// </summary>
    public static object? Coerce(this Type targetType, object? value, TypeCoercionOptions? options = null)
        => TypeCoercer.Coerce(value, targetType, options ?? TypeCoercionOptions.Default);

    /// <summary>
    /// Coerces <paramref name="value"/> to <paramref name="targetType"/> and returns the target type default value on failure.
    /// </summary>
    public static object? CoerceOrDefault(this Type targetType, object? value, TypeCoercionOptions? options = null)
        => TypeCoercer.CoerceOrDefault(value, targetType, options ?? TypeCoercionOptions.Default);

    /// <summary>
    /// Coerces <paramref name="value"/> to <paramref name="targetType"/> and returns <c>null</c> on failure.
    /// </summary>
    public static object? CoerceOrNull(this Type targetType, object? value, TypeCoercionOptions? options = null)
        => TypeCoercer.CoerceOrNull(value, targetType, options ?? TypeCoercionOptions.Default);
}
