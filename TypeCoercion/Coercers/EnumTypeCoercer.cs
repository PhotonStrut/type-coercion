using System;
using System.Globalization;

namespace QueryBuilder.Core.Coercion.Coercers;

internal sealed class EnumTypeCoercer : ITypeCoercer
{
    public CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options)
    {
        if (!effectiveType.IsEnum)
        {
            return CoercionResult.Fail("Type provided must be an Enum.", CoercionErrorCode.UnsupportedSourceType);
        }

        if (value is not string enumString)
        {
            return CoercionResult.Fail(
                $"Enum values must be string names only. Received '{value}' ({value.GetType().Name}) for enum type '{effectiveType.Name}'.",
                CoercionErrorCode.UnsupportedSourceType);
        }

        var candidate = enumString.Trim();
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return CoercionResult.Fail(
                $"Enum values must be string names only. Received empty value for enum type '{effectiveType.Name}'.",
                CoercionErrorCode.InvalidEnumMember);
        }

        if (long.TryParse(candidate, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
        {
            return CoercionResult.Fail(
                $"Enum values must be string names only. Numeric value '{candidate}' is not allowed for enum type '{effectiveType.Name}'.",
                CoercionErrorCode.InvalidEnumMember);
        }

        if (Enum.TryParse(effectiveType, candidate, ignoreCase: true, out var parsed))
            return CoercionResult.Ok(parsed);

        return CoercionResult.Fail(
            $"'{candidate}' is not a valid member of enum type '{effectiveType.Name}'.",
            CoercionErrorCode.InvalidEnumMember);
    }
}
