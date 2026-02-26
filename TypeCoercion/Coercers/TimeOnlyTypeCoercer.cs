using System;
using System.Globalization;

namespace TypeCoercion.Coercers;

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
                return CoercionResult.Fail("The provided string is not a valid TimeOnly.", CoercionErrorCode.InvalidFormat);
            }

            return fallbackCoercer.TryCoerce(value, effectiveType, declaredType, options);
        }
        catch (Exception ex)
        {
            return TypeCoercer.CreateFailureFromException(value, declaredType, ex);
        }
    }
}
