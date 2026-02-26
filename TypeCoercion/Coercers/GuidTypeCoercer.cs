using System;

namespace QueryBuilder.Core.Coercion.Coercers;

internal sealed class GuidTypeCoercer(ITypeCoercer fallbackCoercer) : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (effectiveType != typeof(Guid))
            return CoercionResult.Fail("Type is not Guid.", CoercionErrorCode.UnsupportedSourceType);

        try
        {
            if (value is string guidString)
                return CoercionResult.Ok(Guid.Parse(guidString));

            return fallbackCoercer.TryCoerce(value, effectiveType, declaredType, options);
        }
        catch (Exception ex)
        {
            return TypeCoercion.CreateFailureFromException(value, declaredType, ex);
        }
    }
}
