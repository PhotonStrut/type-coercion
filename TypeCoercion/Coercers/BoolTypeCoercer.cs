using System;

namespace TypeCoercion.Coercers;

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

        if (value is string boolString)
        {
            if (bool.TryParse(boolString, out var boolValue))
                return CoercionResult.Ok(boolValue);
            return CoercionResult.Fail(
                "The provided string is not a valid Boolean.",
                CoercionErrorCode.InvalidFormat);
        }

        try
        {
            return CoercionResult.Ok(Convert.ToBoolean(value, options.Culture));
        }
        catch (Exception ex)
        {
            return TypeCoercer.CreateFailureFromException(value, declaredType, ex);
        }
    }
}
