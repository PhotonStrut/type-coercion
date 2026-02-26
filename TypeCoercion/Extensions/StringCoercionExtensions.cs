using System;

namespace QueryBuilder.Core.Coercion.Extensions;

public static class StringCoercionExtensions
{
    public static T? CoerceTo<T>(this string? value, TypeCoercionOptions? options = null)
        => TypeCoercion.Coerce<T>(value, options ?? TypeCoercionOptions.Default);

    public static CoercionResult<T> TryCoerceTo<T>(this string? value, TypeCoercionOptions? options = null)
        => TypeCoercion.TryCoerce<T>(value, options ?? TypeCoercionOptions.Default);

    public static bool TryCoerceTo<T>(this string? value, out T? result, TypeCoercionOptions? options = null)
    {
        var coercionResult = TypeCoercion.TryCoerce<T>(value, options ?? TypeCoercionOptions.Default);
        result = coercionResult.Success ? (coercionResult.Value == null ? default : (T)coercionResult.Value) : default;
        return coercionResult.Success;
    }
}
