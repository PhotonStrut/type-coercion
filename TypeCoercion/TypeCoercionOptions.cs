using System.Collections.Generic;
using System.Globalization;
using TypeCoercion.Coercers;

namespace TypeCoercion;

/// <summary>
/// Options for configuring type coercion behavior.
/// </summary>
public class TypeCoercionOptions
{
    /// <summary>
    /// The collection of coercers to use. Evaluated in order.
    /// By default, this is pre-populated with the built-in coercers.
    /// </summary>
    public IList<ITypeCoercer> Coercers { get; } = CreateDefaultCoercers();

    private static List<ITypeCoercer> CreateDefaultCoercers()
    {
        var fallbackCoercer = new FallbackTypeCoercer();

        return
        [
            // Basic Types
            new StringTypeCoercer(),
            new BoolTypeCoercer(),
            new GuidTypeCoercer(fallbackCoercer),
            new EnumTypeCoercer(),

            // Date and Time Types
            new DateTimeTypeCoercer(),
            new DateTimeOffsetTypeCoercer(),
            new DateOnlyTypeCoercer(fallbackCoercer),
            new TimeOnlyTypeCoercer(fallbackCoercer),
            new TimeSpanTypeCoercer(fallbackCoercer),

            // Numeric Types
            new NumericTypeCoercer(),

            // Always put the Fallback coercer at the very end
            fallbackCoercer,
        ];
    }

    /// <summary>
    /// The culture to use for parsing/formatting operations. Defaults to InvariantCulture.
    /// </summary>
    public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

    /// <summary>
    /// When true, numeric coercion uses optimized `.TryParse` fast-paths for string inputs.
    /// Note: These fast-paths treat overflow failures as standard format failures (<see cref="CoercionErrorCode.InvalidFormat"/>).
    /// When false, numeric coercion uses <see cref="System.Convert.ChangeType(object, System.Type, System.IFormatProvider)"/>, delivering precise error codes 
    /// (e.g., <see cref="CoercionErrorCode.Overflow"/>) at the cost of performance on invalid input. 
    /// Defaults to true.
    /// </summary>
    public bool UseFastNumericParsing { get; set; } = true;

    /// <summary>
    /// A shared, thread-safe default instance used when no options are provided.
    /// </summary>
    public static TypeCoercionOptions Default { get; } = new TypeCoercionOptions();
}
