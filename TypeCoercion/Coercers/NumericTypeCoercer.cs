using System;

namespace TypeCoercion.Coercers;

internal sealed class NumericTypeCoercer : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (!TypeCoercion.IsNumericType(effectiveType))
            return CoercionResult.Fail("Type is not numeric.", CoercionErrorCode.UnsupportedSourceType);

        if (options.UseFastNumericParsing && value is string stringValue)
        {
            if (effectiveType == typeof(int))
            {
                if (int.TryParse(stringValue, System.Globalization.NumberStyles.Any, options.Culture, out var intValue)) return CoercionResult.Ok(intValue);
                return CoercionResult.Fail("The provided string is not a valid integer.", CoercionErrorCode.InvalidFormat);
            }
            if (effectiveType == typeof(long))
            {
                if (long.TryParse(stringValue, System.Globalization.NumberStyles.Any, options.Culture, out var longValue)) return CoercionResult.Ok(longValue);
                return CoercionResult.Fail("The provided string is not a valid long.", CoercionErrorCode.InvalidFormat);
            }
            if (effectiveType == typeof(double))
            {
                if (double.TryParse(stringValue, System.Globalization.NumberStyles.Any, options.Culture, out var doubleValue)) return CoercionResult.Ok(doubleValue);
                return CoercionResult.Fail("The provided string is not a valid double.", CoercionErrorCode.InvalidFormat);
            }
            if (effectiveType == typeof(decimal))
            {
                if (decimal.TryParse(stringValue, System.Globalization.NumberStyles.Any, options.Culture, out var decimalValue)) return CoercionResult.Ok(decimalValue);
                return CoercionResult.Fail("The provided string is not a valid decimal.", CoercionErrorCode.InvalidFormat);
            }
            if (effectiveType == typeof(float))
            {
                if (float.TryParse(stringValue, System.Globalization.NumberStyles.Any, options.Culture, out var floatValue)) return CoercionResult.Ok(floatValue);
                return CoercionResult.Fail("The provided string is not a valid float.", CoercionErrorCode.InvalidFormat);
            }
            if (effectiveType == typeof(short))
            {
                if (short.TryParse(stringValue, System.Globalization.NumberStyles.Any, options.Culture, out var shortValue)) return CoercionResult.Ok(shortValue);
                return CoercionResult.Fail("The provided string is not a valid short.", CoercionErrorCode.InvalidFormat);
            }
        }

        try
        {
            return CoercionResult.Ok(Convert.ChangeType(value, effectiveType, options.Culture));
        }
        catch (Exception ex)
        {
            return TypeCoercion.CreateFailureFromException(value, declaredType, ex);
        }
    }
}
