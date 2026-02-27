# TypeCoercion

A lightweight, extensible, and high-performance type coercion library for .NET 10. `TypeCoercion` allows you to safely and predictably convert values between mismatched types, providing robust error handling and customizable conversion rules.

## Features

- **Safe Defaults**: `TryCoerce` methods return detailed `CoercionResult` objects instead of throwing exceptions.
- **Throwing Overloads**: `Coerce` methods throw `TypeCoercionException` with structured error codes when coercion fails.
- **Built-In Coercers**: Handles `string`, `bool`, `enum`, all numeric types, `Guid`, `DateTime`, `DateTimeOffset`, `DateOnly`, `TimeOnly`, and `TimeSpan` out of the box.
- **Native JSON Support**: Coerces `System.Text.Json.JsonElement` values directly, including numeric extraction and structural deserialization of objects/arrays.
- **Extension Methods**: Fluent coercion extensions for runtime `Type`, `string`, `JsonElement`, and `IDictionary<string, object?>`.
- **Fallback Helpers**: `CoerceOrDefault`, `CoerceOrNull`, and `CoerceOrFallback` overloads for resilient conversions without exceptions.
- **Extensible**: Inject custom coercion logic by implementing `ITypeCoercer`.
- **High Performance**: Avoids heap allocations in critical paths, uses `TryParse` fast-paths for string-to-number conversions, and avoids enumerator boxing.

## Requirements

- .NET 10.0 or later

## Installation

```bash
dotnet add package TypeCoercion
```

## Basic Usage

### Safe Coercion (no exceptions)

`TryCoerce` returns a `CoercionResult<T>` with `Success`, `Value`, `Error`, and `ErrorCode` properties.

```csharp
using TypeCoercion;

var result = TypeCoercer.TryCoerce<int>("42");

if (result.Success)
{
    Console.WriteLine($"Number: {result.Value}"); // Output: Number: 42
}

var invalid = TypeCoercer.TryCoerce<Guid>("not-a-guid");

if (!invalid.Success)
{
    Console.WriteLine($"Error: {invalid.Error}");
    Console.WriteLine($"Code: {invalid.ErrorCode}"); // InvalidFormat
}
```

### Throwing Coercion

`Coerce` throws a `TypeCoercionException` (with an `ErrorCode` property) on failure.

```csharp
using TypeCoercion;

DateTime date = TypeCoercer.Coerce<DateTime>("2026-01-01T12:00:00Z");

try
{
    int number = TypeCoercer.Coerce<int>("not_a_number");
}
catch (TypeCoercionException ex)
{
    Console.WriteLine($"{ex.Message} (Code: {ex.ErrorCode})");
}
```

### Fallback Coercion

Use fallback helpers when you want non-throwing coercion with a direct value result.

```csharp
using TypeCoercion;

int number = TypeCoercer.CoerceOrDefault<int>("bad");            // 0
int? nullable = TypeCoercer.CoerceOrNull<int?>("bad");           // null
int chosen = TypeCoercer.CoerceOrFallback("bad", fallbackValue: 7); // 7

// Note: use nullable targets with CoerceOrNull<T>() for value types.
// CoerceOrNull<int>("bad") throws because int cannot represent null.
```

### Non-Generic Overloads

When the target type is only known at runtime:

```csharp
CoercionResult result = TypeCoercer.TryCoerce("42", typeof(int));
object? value = TypeCoercer.Coerce("42", typeof(int));
object? fallbackDefault = TypeCoercer.CoerceOrDefault("bad", typeof(int)); // 0 (boxed)
object? fallbackNull = TypeCoercer.CoerceOrNull("bad", typeof(Guid));       // null
```

## Extension Methods

Fluent extensions are available in the `TypeCoercion.Extensions` namespace.

### String Extensions

```csharp
using TypeCoercion.Extensions;

int number = "42".CoerceTo<int>();
CoercionResult<int> result = "42".TryCoerceTo<int>();
int defaulted = "bad".CoerceToOrDefault<int>();     // 0
int? nulled = "bad".CoerceToOrNull<int?>();         // null
int fallback = "bad".CoerceToOrFallback(5);         // 5

if ("42".TryCoerceTo<int>(out int parsed))
{
    Console.WriteLine(parsed);
}
```

### Type Extensions

```csharp
using TypeCoercion.Extensions;

Type intType = typeof(int);

object? value = intType.Coerce("42");             // 42 (boxed)
object? defaulted = intType.CoerceOrDefault("x"); // 0 (boxed)
object? nulled = intType.CoerceOrNull("x");       // null
```

### JsonElement Extensions

```csharp
using TypeCoercion.Extensions;

JsonElement element = GetJsonElement();

var dto = element.CoerceTo<MyDto>();
CoercionResult<int> result = element.TryCoerceTo<int>();
```

### Dictionary Extensions

Useful for coercing loosely-typed dictionary payloads (e.g., from form data or dynamic configurations).

```csharp
using TypeCoercion.Extensions;

IDictionary<string, object?> data = GetData();

int id = data.CoerceValue<int>("id");                    // throws on missing key or coercion failure
int port = data.CoerceValueOrDefault<int>("port", 8080); // returns default on missing key or failure
CoercionResult<int> result = data.TryCoerceValue<int>("id");
```

## JSON Element Coercion

`TypeCoercion` handles `System.Text.Json.JsonElement` natively:

- **String/Bool/Null values** are unwrapped and coerced through the standard pipeline.
- **Number values** use type-specific `TryGet*()` methods (e.g., `TryGetInt32`, `TryGetDecimal`) for precise extraction without string round-tripping.
- **Objects and arrays** are deserialized via `JsonSerializer.Deserialize` with case-insensitive property matching.

```csharp
JsonElement element = GetJsonFromSomewhere();

var result = TypeCoercer.TryCoerce<MyCustomDto>(element);
```

## Error Codes

Every `CoercionResult` and `TypeCoercionException` carries a `CoercionErrorCode`:

| Code | Meaning |
|------|---------|
| `None` | No error (successful result). |
| `InvalidFormat` | The value's format is invalid for the target type (e.g., `"abc"` to `int`). |
| `Overflow` | The value exceeds the range of the target type. |
| `UnsupportedSourceType` | The source type is not supported for the target type. |
| `InvalidEnumMember` | The value is not a valid member of the target enum. |
| `ConversionFailed` | General failure that doesn't fit a more specific category. |

## Customizing Behavior

### TypeCoercionOptions

Control coercion behavior by passing a `TypeCoercionOptions` instance:

```csharp
var options = new TypeCoercionOptions
{
    Culture = new CultureInfo("fr-FR"),
    UseFastNumericParsing = false
};

var result = TypeCoercer.TryCoerce<decimal>("12,5", options);
```

| Property | Default | Description |
|----------|---------|-------------|
| `Culture` | `InvariantCulture` | The `CultureInfo` used for parsing and formatting. |
| `UseFastNumericParsing` | `true` | When `true`, uses `TryParse` fast-paths for string-to-number coercion. Overflow is reported as `InvalidFormat`. When `false`, uses `Convert.ChangeType` for precise error codes (e.g., `Overflow`) at a performance cost. |
| `Coercers` | Built-in set | The ordered list of `ITypeCoercer` instances. Pre-populated with all built-in coercers. |

### Custom Coercers

Implement `ITypeCoercer` and insert it into the coercer pipeline:

```csharp
public interface ITypeCoercer
{
    CoercionResult TryCoerce(
        object value,
        Type effectiveType,  // Underlying type (e.g., int for int?)
        Type declaredType,   // Originally declared type (e.g., int?)
        TypeCoercionOptions options);
}
```

Return `CoercionResult.Ok(value)` on success, `CoercionResult.Fail(...)` with `UnsupportedSourceType` to pass through to the next coercer, or any other error code to halt the pipeline with a failure.

```csharp
var options = new TypeCoercionOptions();
options.Coercers.Insert(0, new MyCustomCoercer()); // runs before built-in coercers

var result = TypeCoercer.TryCoerce<MyType>(value, options);
```

## Built-In Coercer Behavior

| Target Type | Accepted Sources | Notes |
|-------------|-----------------|-------|
| `string` | Any | Uses `Convert.ToString()`. |
| `bool` | `string`, numeric types | String via `bool.TryParse`; numeric via `Convert.ToBoolean` (`1` → `true`, `0` → `false`). |
| `enum` | `string` only | Case-insensitive name matching. Numeric values are rejected to prevent mass-assignment vulnerabilities. |
| `Guid` | `string` | Via `Guid.TryParse`. |
| `DateTime` | `string`, `DateTimeOffset`, `DateOnly` | Culture-aware parsing. |
| `DateTimeOffset` | `string`, `DateTime`, `DateOnly` | Culture-aware parsing. |
| `DateOnly` | `string`, `DateTime`, `DateTimeOffset` | Culture-aware parsing. |
| `TimeOnly` | `string` | Culture-aware parsing. |
| `TimeSpan` | `string` | Culture-aware parsing. |
| Numeric types | `string`, other numeric types | `TryParse` fast-path for `int`, `long`, `double`, `decimal`, `float`, `short`; `Convert.ChangeType` fallback for others. |
| Any other | Any | Falls back to `Convert.ChangeType()`. |

## License

This project is licensed under the [MIT License](LICENSE).
