using System;

namespace QueryBuilder.Core.Coercion.Coercers;

internal sealed class FallbackTypeCoercer : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
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
