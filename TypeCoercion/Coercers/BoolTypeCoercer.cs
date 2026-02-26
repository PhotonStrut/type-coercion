using System;

namespace QueryBuilder.Core.Coercion.Coercers;

/// <remarks>
/// Accepts numeric conversions via <see cref="Convert.ToBoolean(object, IFormatProvider)"/>
/// (e.g., <c>1</c> → <c>true</c>, <c>0</c> → <c>false</c>). This is intentionally different
/// from enum coercion, which rejects numeric values to prevent mass-assignment attacks.
/// </remarks>
internal sealed class BoolTypeCoercer : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (effectiveType != typeof(bool))
            return CoercionResult.Fail("Type is not bool.", CoercionErrorCode.UnsupportedSourceType);

        try
        {
            if (value is string boolString)
                return CoercionResult.Ok(bool.Parse(boolString));

            return CoercionResult.Ok(Convert.ToBoolean(value, options.Culture));
        }
        catch (Exception ex)
        {
            return TypeCoercion.CreateFailureFromException(value, declaredType, ex);
        }
    }
}
