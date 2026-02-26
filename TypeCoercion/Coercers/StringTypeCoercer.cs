using System;

namespace TypeCoercion.Coercers;

internal sealed class StringTypeCoercer : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (effectiveType != typeof(string))
            return CoercionResult.Fail("Type is not string.", CoercionErrorCode.UnsupportedSourceType);

        return CoercionResult.Ok(Convert.ToString(value, options.Culture) ?? value.ToString());
    }
}
