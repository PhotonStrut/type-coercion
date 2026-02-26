using System;

namespace QueryBuilder.Core.Coercion.Coercers;

internal sealed class NumericTypeCoercer : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (!TypeCoercion.IsNumericType(effectiveType))
            return CoercionResult.Fail("Type is not numeric.", CoercionErrorCode.UnsupportedSourceType);

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
