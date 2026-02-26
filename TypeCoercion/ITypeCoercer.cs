using System;

namespace QueryBuilder.Core.Coercion;

public interface ITypeCoercer
{
    CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options);
}
