using System;

namespace QueryBuilder.Core.Coercion;

/// <summary>
/// Defines a contract for coercing a value to a specific type.
/// </summary>
public interface ITypeCoercer
{
    /// <summary>
    /// Attempts to coerce the given value to the effective type.
    /// </summary>
    /// <param name="value">The value to coerce.</param>
    /// <param name="effectiveType">The actual underlying type being targeted (e.g., T instead of Nullable&lt;T&gt;).</param>
    /// <param name="declaredType">The originally declared target type.</param>
    /// <param name="options">Options for type coercion.</param>
    /// <returns>A <see cref="CoercionResult"/> indicating success or failure.</returns>
    CoercionResult TryCoerce(object value, Type effectiveType, Type declaredType, TypeCoercionOptions options);
}
