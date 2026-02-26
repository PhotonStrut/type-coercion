using System;
using System.Globalization;

namespace QueryBuilder.Core.Coercion.Coercers;

internal sealed class DateOnlyTypeCoercer(ITypeCoercer fallbackCoercer) : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (effectiveType != typeof(DateOnly))
            return CoercionResult.Fail("Type is not DateOnly.", CoercionErrorCode.UnsupportedSourceType);

        try
        {
            if (value is string dateOnlyString)
                return CoercionResult.Ok(DateOnly.Parse(dateOnlyString, options.Culture));
            if (value is DateTime dateTime)
                return CoercionResult.Ok(DateOnly.FromDateTime(dateTime));
            if (value is DateTimeOffset dateTimeOffset)
                return CoercionResult.Ok(DateOnly.FromDateTime(dateTimeOffset.DateTime));

            return fallbackCoercer.TryCoerce(value, effectiveType, declaredType, options);
        }
        catch (Exception ex)
        {
            return TypeCoercion.CreateFailureFromException(value, declaredType, ex);
        }
    }
}
