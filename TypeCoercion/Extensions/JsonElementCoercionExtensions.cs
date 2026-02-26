using System.Text.Json;

namespace TypeCoercion.Extensions;

/// <summary>
/// Extension methods for coercing <see cref="JsonElement"/> values.
/// </summary>
public static class JsonElementCoercionExtensions
{
    /// <summary>
    /// Coerces a <see cref="JsonElement"/> to the specified type, throwing on failure.
    /// </summary>
    public static T? CoerceTo<T>(this JsonElement element, TypeCoercionOptions? options = null)
        => TypeCoercer.Coerce<T>(element, options ?? TypeCoercionOptions.Default);

    /// <summary>
    /// Attempts to coerce a <see cref="JsonElement"/> to the specified type without throwing.
    /// </summary>
    public static CoercionResult<T> TryCoerceTo<T>(this JsonElement element, TypeCoercionOptions? options = null)
        => TypeCoercer.TryCoerce<T>(element, options ?? TypeCoercionOptions.Default);

    /// <summary>
    /// Attempts to coerce a <see cref="JsonElement"/> to the specified type and outputs the result.
    /// </summary>
    public static bool TryCoerceTo<T>(this JsonElement element, out T? result, TypeCoercionOptions? options = null)
    {
        var coercionResult = TypeCoercer.TryCoerce<T>(element, options ?? TypeCoercionOptions.Default);
        result = coercionResult.Success ? coercionResult.Value : default;
        return coercionResult.Success;
    }
}
