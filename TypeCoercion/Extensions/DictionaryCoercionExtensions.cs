using System.Collections.Generic;

namespace QueryBuilder.Core.Coercion.Extensions;

/// <summary>
/// Extension methods for coercing values from dictionaries.
/// </summary>
public static class DictionaryCoercionExtensions
{
    /// <summary>
    /// Coerces a dictionary value by key to the specified type, throwing on failure or missing key.
    /// </summary>
    public static T? CoerceValue<T>(this IDictionary<string, object?> dict, string key, TypeCoercionOptions? options = null)
    {
        if (!dict.TryGetValue(key, out var value))
            throw new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.");
            
        return TypeCoercion.Coerce<T>(value, options ?? TypeCoercionOptions.Default);
    }

    /// <summary>
    /// Coerces a dictionary value by key to the specified type, returning a default value on failure or missing key.
    /// </summary>
    public static T? CoerceValueOrDefault<T>(this IDictionary<string, object?> dict, string key, T? defaultValue = default, TypeCoercionOptions? options = null)
    {
        if (!dict.TryGetValue(key, out var value))
            return defaultValue;
            
        var result = TypeCoercion.TryCoerce<T>(value, options ?? TypeCoercionOptions.Default);
        if (result.Success)
        {
             return result.Value == null ? default : (T)result.Value;
        }
        return defaultValue;
    }

    /// <summary>
    /// Attempts to coerce a dictionary value by key to the specified type without throwing.
    /// </summary>
    public static CoercionResult<T> TryCoerceValue<T>(this IDictionary<string, object?> dict, string key, TypeCoercionOptions? options = null)
    {
        if (!dict.TryGetValue(key, out var value))
            return CoercionResult<T>.Fail($"The given key '{key}' was not present in the dictionary.", CoercionErrorCode.UnsupportedSourceType);
            
        return TypeCoercion.TryCoerce<T>(value, options ?? TypeCoercionOptions.Default);
    }
}
