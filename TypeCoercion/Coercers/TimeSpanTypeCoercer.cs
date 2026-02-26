using System;
using System.Globalization;

namespace QueryBuilder.Core.Coercion.Coercers;

internal sealed class TimeSpanTypeCoercer(ITypeCoercer fallbackCoercer) : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (effectiveType != typeof(TimeSpan))
            return CoercionResult.Fail("Type is not TimeSpan.", CoercionErrorCode.UnsupportedSourceType);

        try
        {
            if (value is string timeSpanString)
            {
                if (TimeSpan.TryParse(timeSpanString, options.Culture, out var timeSpan))
                    return CoercionResult.Ok(timeSpan);
                return CoercionResult.Fail($"String '{timeSpanString}' is not a valid TimeSpan.", CoercionErrorCode.InvalidFormat);
            }

            return fallbackCoercer.TryCoerce(value, effectiveType, declaredType, options);
        }
        catch (Exception ex)
        {
            return TypeCoercion.CreateFailureFromException(value, declaredType, ex);
        }
    }
}
