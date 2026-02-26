using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using QueryBuilder.Core.Coercion.Coercers;

namespace QueryBuilder.Core.Coercion;

/// <summary>
/// Pure static utility for coercing values between CLR types.
/// </summary>
public static class TypeCoercion
{
    private static readonly Type[] NumericTypes =
    [
        typeof(byte), typeof(sbyte),
        typeof(short), typeof(ushort),
        typeof(int), typeof(uint),
        typeof(long), typeof(ulong),
        typeof(float), typeof(double),
        typeof(decimal)
    ];

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    static TypeCoercion()
    {
        // Populate the default options with the built-in coercers
        var options = TypeCoercionOptions.Default;
        
        var fallbackCoercer = new FallbackTypeCoercer();
        
        // Order matters for some of these if we ever change dispatch logic, 
        // though currently we dispatch by type exactly.
        
        // Basic Types
        options.Coercers.Add(new StringTypeCoercer());
        options.Coercers.Add(new BoolTypeCoercer());
        options.Coercers.Add(new GuidTypeCoercer(fallbackCoercer));
        options.Coercers.Add(new EnumTypeCoercer());
        
        // Date and Time Types
        options.Coercers.Add(new DateTimeTypeCoercer());
        options.Coercers.Add(new DateTimeOffsetTypeCoercer());
        options.Coercers.Add(new DateOnlyTypeCoercer(fallbackCoercer));
        options.Coercers.Add(new TimeOnlyTypeCoercer(fallbackCoercer));
        options.Coercers.Add(new TimeSpanTypeCoercer(fallbackCoercer));

        // Numeric Types (shared coercer instance)
        options.Coercers.Add(new NumericTypeCoercer());

        // Always put the Fallback coercer at the very end
        options.Coercers.Add(fallbackCoercer);
    }

    /// <summary>
    /// Attempts to coerce <paramref name="value"/> to <paramref name="targetType"/> without throwing using default options.
    /// </summary>
    public static CoercionResult TryCoerce(object? value, Type targetType)
        => TryCoerce(value, targetType, TypeCoercionOptions.Default);

    /// <summary>
    /// Coerces <paramref name="value"/> to <paramref name="targetType"/>, throwing on failure, using default options.
    /// </summary>
    public static object? Coerce(object? value, Type targetType)
        => Coerce(value, targetType, TypeCoercionOptions.Default);

    /// <summary>
    /// Attempts to coerce <paramref name="value"/> to <paramref name="targetType"/> without throwing.
    /// </summary>
    public static CoercionResult TryCoerce(object? value, Type targetType, TypeCoercionOptions options)
    {
        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentNullException.ThrowIfNull(options);

        if (value is JsonElement jsonElement && targetType != typeof(JsonElement))
        {
            return TryCoerceJsonElement(jsonElement, targetType, options);
        }

        if (value is null)
        {
            if (IsNullableTargetType(targetType))
                return CoercionResult.Ok(null);

            return CoercionResult.Fail(
                $"Cannot convert null to non-nullable type '{targetType.Name}'.",
                CoercionErrorCode.ConversionFailed);
        }

        var declaredType = targetType;
        var effectiveType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (effectiveType.IsInstanceOfType(value))
            return CoercionResult.Ok(value);

        // Before, it was a dictionary lookup. Now, let's try mapping known types to the built-in coercers,
        // but FIRST let's check the options.Coercers for any custom overrides.
        // Actually, the new design specifies building a Dictionary internally or checking types directly,
        // but consumers can insert into `options.Coercers`.
        // Since `ITypeCoercer` doesn't have `CanHandle`, if a custom coercer can't handle it, what does it return? 
        // It returns a Failure Result. We shouldn't stop at the first failure if it's just unsupported.
        // But `UnsupportedSourceType` is an error code. 
        // We will execute `options.Coercers` in order. If it returns Success, we are done.
        // If it returns a Failure that is NOT UnsupportedSourceType, we probably still want to fail?
        // Actually, the simplest approach is just to map the built-ins based on type, just like before,
        // but we evaluate custom ones first.
        
        // Wait, the original code had:
        // if (Coercers.TryGetValue(effectiveType, out var coercer)) return coercer.TryCoerce(...);
        // Let's implement that exact mapping internally.
        
        return DispatchToCoercer(value, effectiveType, declaredType, options);
    }
    
    private static CoercionResult DispatchToCoercer(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (effectiveType.IsEnum)
        {
            // Specifically route enums to the EnumTypeCoercer if one exists
            foreach (var coercer in options.Coercers)
            {
                if (coercer is EnumTypeCoercer)
                    return coercer.TryCoerce(value, effectiveType, declaredType, options);
            }
        }

        // 1. Check custom and built-in coercers
        foreach (var coercer in options.Coercers)
        {
            // Skip the EnumTypeCoercer here to prevent ArgumentException on non-enum types
            if (coercer is EnumTypeCoercer && !effectiveType.IsEnum) continue;

            var result = coercer.TryCoerce(value, effectiveType, declaredType, options);
            
            // If the coercer explicitly returns success, return it immediately
            if (result.Success) return result;
            
            // If the coercer returns a specific failure that is NOT UnsupportedSourceType, bubble it up.
            // UnsupportedSourceType indicates the coercer simply doesn't handle this combination.
            if (result.ErrorCode != CoercionErrorCode.UnsupportedSourceType)
                return result;
        }

        return CoercionResult.Fail(
            $"No coercer found for source type '{value.GetType().Name}' and target type '{effectiveType.Name}'.",
            CoercionErrorCode.ConversionFailed);
    }

    /// <summary>
    /// Coerces <paramref name="value"/> to <paramref name="targetType"/>, throwing on failure.
    /// </summary>
    public static object? Coerce(object? value, Type targetType, TypeCoercionOptions options)
    {
        var result = TryCoerce(value, targetType, options);
        if (result.Success)
            return result.Value;

        throw result.OriginalException is not null
            ? new TypeCoercionException(result.Error, result.ErrorCode, result.OriginalException)
            : new TypeCoercionException(result.Error, result.ErrorCode);
    }

    private static CoercionResult TryCoerceJsonElement(JsonElement jsonElement, Type targetType, TypeCoercionOptions options)
    {
        return jsonElement.ValueKind switch
        {
            JsonValueKind.String => TryCoerce(jsonElement.GetString(), targetType, options),
            JsonValueKind.Number => TryCoerceJsonNumber(jsonElement, targetType, options),
            JsonValueKind.True => TryCoerce(true, targetType, options),
            JsonValueKind.False => TryCoerce(false, targetType, options),
            JsonValueKind.Null => TryCoerce(null, targetType, options),
            JsonValueKind.Object or JsonValueKind.Array => TryDeserializeComplexJson(jsonElement, targetType),
            _ => CoercionResult.Fail(
                $"Cannot convert JSON value kind '{jsonElement.ValueKind}' to type '{targetType.Name}'.",
                CoercionErrorCode.UnsupportedSourceType)
        };
    }

    private static CoercionResult TryCoerceJsonNumber(JsonElement jsonElement, Type targetType, TypeCoercionOptions options)
    {
        var effectiveType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (effectiveType == typeof(byte) && jsonElement.TryGetByte(out var byteValue))
            return TryCoerce(byteValue, targetType, options);

        if (effectiveType == typeof(sbyte) && jsonElement.TryGetSByte(out var sbyteValue))
            return TryCoerce(sbyteValue, targetType, options);

        if (effectiveType == typeof(short) && jsonElement.TryGetInt16(out var shortValue))
            return TryCoerce(shortValue, targetType, options);

        if (effectiveType == typeof(ushort) && jsonElement.TryGetUInt16(out var ushortValue))
            return TryCoerce(ushortValue, targetType, options);

        if (effectiveType == typeof(int) && jsonElement.TryGetInt32(out var intValue))
            return TryCoerce(intValue, targetType, options);

        if (effectiveType == typeof(uint) && jsonElement.TryGetUInt32(out var uintValue))
            return TryCoerce(uintValue, targetType, options);

        if (effectiveType == typeof(long) && jsonElement.TryGetInt64(out var longValue))
            return TryCoerce(longValue, targetType, options);

        if (effectiveType == typeof(ulong) && jsonElement.TryGetUInt64(out var ulongValue))
            return TryCoerce(ulongValue, targetType, options);

        if (effectiveType == typeof(decimal) && jsonElement.TryGetDecimal(out var decimalValue))
            return TryCoerce(decimalValue, targetType, options);

        if (effectiveType == typeof(double) && jsonElement.TryGetDouble(out var doubleValue))
            return TryCoerce(doubleValue, targetType, options);

        if (effectiveType == typeof(float) && jsonElement.TryGetSingle(out var floatValue))
            return TryCoerce(floatValue, targetType, options);

        return TryCoerce(jsonElement.GetRawText(), targetType, options);
    }

    private static CoercionResult TryDeserializeComplexJson(JsonElement jsonElement, Type targetType)
    {
        var effectiveType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (!IsComplexType(effectiveType))
        {
            return CoercionResult.Fail(
                $"Cannot convert JSON {jsonElement.ValueKind.ToString().ToLowerInvariant()} to non-complex type '{targetType.Name}'.",
                CoercionErrorCode.UnsupportedSourceType);
        }

        try
        {
            var deserialized = JsonSerializer.Deserialize(
                jsonElement.GetRawText(),
                effectiveType,
                JsonSerializerOptions);

            if (deserialized is null)
            {
                if (IsNullableTargetType(targetType))
                    return CoercionResult.Ok(null);

                return CoercionResult.Fail(
                    $"Cannot deserialize JSON {jsonElement.ValueKind.ToString().ToLowerInvariant()} to non-nullable type '{targetType.Name}'.",
                    CoercionErrorCode.ConversionFailed);
            }

            return CoercionResult.Ok(deserialized);
        }
        catch (Exception ex)
        {
            return CreateFailureFromException(jsonElement.GetRawText(), targetType, ex);
        }
    }

    private static bool IsNullableTargetType(Type targetType)
        => !targetType.IsValueType || Nullable.GetUnderlyingType(targetType) is not null;

    private static bool IsComplexType(Type targetType)
    {
        if (targetType == typeof(object))
            return false;

        if (targetType.IsPrimitive || targetType.IsEnum)
            return false;

        if (targetType == typeof(string) ||
            targetType == typeof(decimal) ||
            targetType == typeof(DateTime) ||
            targetType == typeof(DateTimeOffset) ||
            targetType == typeof(DateOnly) ||
            targetType == typeof(TimeOnly) ||
            targetType == typeof(TimeSpan) ||
            targetType == typeof(Guid))
            return false;

        return true;
    }

    internal static CoercionResult CreateFailureFromException(object value, Type targetType, Exception ex)
    {
        var displayValue = value.ToString();
        if (!string.IsNullOrEmpty(displayValue) && displayValue.Length > 50)
            displayValue = displayValue[..50] + "...";

        return CoercionResult.Fail(
            $"Cannot convert '{displayValue}' ({value.GetType().Name}) to type '{targetType.Name}': {ex.Message}",
            GetErrorCode(ex),
            ex);
    }

    private static CoercionErrorCode GetErrorCode(Exception ex) => ex switch
    {
        FormatException => CoercionErrorCode.InvalidFormat,
        JsonException => CoercionErrorCode.InvalidFormat,
        OverflowException => CoercionErrorCode.Overflow,
        NotSupportedException => CoercionErrorCode.UnsupportedSourceType,
        _ => CoercionErrorCode.ConversionFailed
    };
}
