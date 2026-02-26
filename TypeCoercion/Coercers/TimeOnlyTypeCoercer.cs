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
                return CoercionResult.Ok(TimeOnly.Parse(timeString, options.Culture));

            return fallbackCoercer.TryCoerce(value, effectiveType, declaredType, options);
        }
        catch (Exception ex)
        {
            return TypeCoercion.CreateFailureFromException(value, declaredType, ex);
        }
    }
}
