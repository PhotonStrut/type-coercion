using System;
using System.Globalization;

namespace QueryBuilder.Core.Coercion.Coercers;

internal sealed class DateTimeOffsetTypeCoercer : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (effectiveType != typeof(DateTimeOffset))
            return CoercionResult.Fail("Type is not DateTimeOffset.", CoercionErrorCode.UnsupportedSourceType);

        try
        {
            if (value is string dateString)
            {
                if (DateTimeOffset.TryParse(dateString, options.Culture, out var dateTimeOffset))
                    return CoercionResult.Ok(dateTimeOffset);
                return CoercionResult.Fail($"String '{dateString}' is not a valid DateTimeOffset.", CoercionErrorCode.InvalidFormat);
            }
            if (value is DateTime dateTime)
                return CoercionResult.Ok(new DateTimeOffset(dateTime));
            if (value is DateOnly dateOnly)
                return CoercionResult.Ok(new DateTimeOffset(dateOnly.ToDateTime(TimeOnly.MinValue)));

            return CoercionResult.Fail(
                $"Cannot convert value of type '{value.GetType().Name}' to '{effectiveType.Name}'. Supported source types: string, DateTime, DateOnly.",
                CoercionErrorCode.UnsupportedSourceType);
        }
        catch (Exception ex)
        {
            return TypeCoercion.CreateFailureFromException(value, declaredType, ex);
        }
    }
}
