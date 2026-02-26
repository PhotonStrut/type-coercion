using System.Collections.Generic;
using System.Globalization;

namespace QueryBuilder.Core.Coercion;

/// <summary>
/// Options for configuring type coercion behavior.
/// </summary>
public class TypeCoercionOptions
{
    /// <summary>
    /// The collection of coercers to use. Evaluated in order.
    /// By default, this is pre-populated with the built-in coercers.
    /// </summary>
    public IList<ITypeCoercer> Coercers { get; } = new List<ITypeCoercer>();

    /// <summary>
    /// The culture to use for parsing/formatting operations. Defaults to InvariantCulture.
    /// </summary>
    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

    /// <summary>
    /// A shared, thread-safe default instance used when no options are provided.
    /// </summary>
    public static TypeCoercionOptions Default { get; } = new TypeCoercionOptions();
}
