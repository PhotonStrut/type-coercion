using System;

namespace TypeCoercion.Coercers;

internal sealed class GuidTypeCoercer(ITypeCoercer fallbackCoercer) : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (effectiveType != typeof(Guid))
            return CoercionResult.Fail("Type is not Guid.", CoercionErrorCode.UnsupportedSourceType);

        try
        {
            if (value is string guidString)
            {
                if (Guid.TryParse(guidString, out var guid))
                    return CoercionResult.Ok(guid);
                return CoercionResult.Fail("The provided string is not a valid Guid.", CoercionErrorCode.InvalidFormat);
            }

            return fallbackCoercer.TryCoerce(value, effectiveType, declaredType, options);
        }
        catch (Exception ex)
        {
            return TypeCoercion.CreateFailureFromException(value, declaredType, ex);
        }
    }
}
