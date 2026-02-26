using System;
using System.Globalization;

namespace QueryBuilder.Core.Coercion.Coercers;

internal sealed class TimeOnlyTypeCoercer(ITypeCoercer fallbackCoercer) : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (effectiveType != typeof(TimeOnly))
            return CoercionResult.Fail("Type is not TimeOnly.", CoercionErrorCode.UnsupportedSourceType);

        try
        {
            if (value is string timeString)
            {
                if (TimeOnly.TryParse(timeString, options.Culture, out var timeOnly))
                    return CoercionResult.Ok(timeOnly);
                return CoercionResult.Fail($"String '{timeString}' is not a valid TimeOnly.", CoercionErrorCode.InvalidFormat);
            }

            return fallbackCoercer.TryCoerce(value, effectiveType, declaredType, options);
        }
        catch (Exception ex)
        {
            return TypeCoercion.CreateFailureFromException(value, declaredType, ex);
        }
    }
}
