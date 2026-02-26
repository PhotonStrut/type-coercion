using System.Text.Json;

namespace QueryBuilder.Core.Coercion.Extensions;

public static class JsonElementCoercionExtensions
{
    public static T? CoerceTo<T>(this JsonElement element, TypeCoercionOptions? options = null)
        => TypeCoercion.Coerce<T>(element, options ?? TypeCoercionOptions.Default);

    public static CoercionResult<T> TryCoerceTo<T>(this JsonElement element, TypeCoercionOptions? options = null)
        => TypeCoercion.TryCoerce<T>(element, options ?? TypeCoercionOptions.Default);

    public static bool TryCoerceTo<T>(this JsonElement element, out T? result, TypeCoercionOptions? options = null)
    {
        var coercionResult = TypeCoercion.TryCoerce<T>(element, options ?? TypeCoercionOptions.Default);
        result = coercionResult.Success ? (coercionResult.Value == null ? default : (T)coercionResult.Value) : default;
        return coercionResult.Success;
    }
}
