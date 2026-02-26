using System;

namespace QueryBuilder.Core.Coercion.Coercers;

internal sealed class NumericTypeCoercer : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (effectiveType != typeof(byte) && effectiveType != typeof(sbyte) &&
            effectiveType != typeof(short) && effectiveType != typeof(ushort) &&
            effectiveType != typeof(int) && effectiveType != typeof(uint) &&
            effectiveType != typeof(long) && effectiveType != typeof(ulong) &&
            effectiveType != typeof(float) && effectiveType != typeof(double) &&
            effectiveType != typeof(decimal))
        {
            return CoercionResult.Fail("Type is not numeric.", CoercionErrorCode.UnsupportedSourceType);
        }

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
