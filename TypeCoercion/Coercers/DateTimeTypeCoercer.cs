using System;
using System.Globalization;

namespace TypeCoercion.Coercers;

internal sealed class DateTimeTypeCoercer : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (effectiveType != typeof(DateTime))
            return CoercionResult.Fail("Type is not DateTime.", CoercionErrorCode.UnsupportedSourceType);

        try
        {
            if (value is string dateString)
            {
                if (DateTime.TryParse(dateString, options.Culture, out var dateTime))
                    return CoercionResult.Ok(dateTime);
                return CoercionResult.Fail("The provided string is not a valid DateTime.", CoercionErrorCode.InvalidFormat);
            }
            if (value is DateTimeOffset dateTimeOffset)
                return CoercionResult.Ok(dateTimeOffset.DateTime);
            if (value is DateOnly dateOnly)
                return CoercionResult.Ok(dateOnly.ToDateTime(TimeOnly.MinValue));

            return CoercionResult.Fail(
                "Cannot convert the provided value to DateTime. Supported source types: string, DateTimeOffset, DateOnly.",
                CoercionErrorCode.UnsupportedSourceType);
        }
        catch (Exception ex)
        {
            return TypeCoercion.CreateFailureFromException(value, declaredType, ex);
        }
    }
}
