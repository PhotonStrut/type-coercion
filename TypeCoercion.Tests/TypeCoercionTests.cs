using System.Text.Json;
using TypeCoercion;
using static TypeCoercion.TypeCoercer;
using Shouldly;

namespace TypeCoercion.Tests;

public sealed class TypeCoercionTests
{
    private sealed class JsonPayload
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    // ── Null handling ──────────────────────────────────────────────────

    [Fact]
    public void Coerce_NullToNonNullableInt_ThrowsTypeCoercionException()
    {
        Should.Throw<TypeCoercionException>(() =>
            Coerce(null, typeof(int)));
    }

    [Fact]
    public void Coerce_NullToNullableInt_ReturnsNull()
    {
        Coerce(null, typeof(int?)).ShouldBeNull();
    }

    // ── Identity check ─────────────────────────────────────────────────

    [Fact]
    public void Coerce_SameType_ReturnsSameValue()
    {
        Coerce(42, typeof(int)).ShouldBe(42);
    }

    [Fact]
    public void CoerceOrDefault_NonGeneric_Success_ReturnsCoercedValue()
    {
        CoerceOrDefault("42", typeof(int)).ShouldBe(42);
    }

    [Fact]
    public void CoerceOrDefault_NonGeneric_Failure_ReturnsTypeDefault()
    {
        CoerceOrDefault("bad", typeof(int)).ShouldBe(0);
    }

    [Fact]
    public void CoerceOrNull_NonGeneric_Failure_ReturnsNull()
    {
        CoerceOrNull("bad", typeof(Guid)).ShouldBeNull();
    }

    [Fact]
    public void CoerceOrDefault_NonGeneric_VoidTarget_Failure_ReturnsNullWithoutThrowing()
    {
        object? value = null;
        Should.NotThrow(() => value = CoerceOrDefault("bad", typeof(void)));
        value.ShouldBeNull();
    }

    // ── Nullable target with non-null value (S6) ───────────────────────

    [Fact]
    public void TryCoerce_StringToNullableInt_Succeeds()
    {
        var result = TryCoerce("42", typeof(int?));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe(42)
        );
    }

    [Fact]
    public void TryCoerce_StringToNullableDateTime_Succeeds()
    {
        var result = TryCoerce("2026-03-15", typeof(DateTime?));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBeOfType<DateTime>()
        );
    }

    [Fact]
    public void TryCoerce_StringToNullableEnum_Succeeds()
    {
        var result = TryCoerce("Tuesday", typeof(DayOfWeek?));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe(DayOfWeek.Tuesday)
        );
    }

    // ── Numeric coercion ───────────────────────────────────────────────

    [Fact]
    public void Coerce_StringToInt_Parses()
    {
        Coerce("123", typeof(int)).ShouldBe(123);
    }

    [Fact]
    public void Coerce_IntToDecimal_Converts()
    {
        var result = Coerce(42, typeof(decimal));
        result.ShouldSatisfyAllConditions(
            () => result.ShouldBeOfType<decimal>(),
            () => result.ShouldBe(42m)
        );
    }

    [Fact]
    public void TryCoerce_InvalidStringToInt_ReturnsInvalidFormat()
    {
        var result = TryCoerce("not-a-number", typeof(int));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.InvalidFormat)
        );
    }

    [Fact]
    public void TryCoerce_IntOverflow_FastParsing_ReturnsInvalidFormat()
    {
        // By default, UseFastNumericParsing is true, which collapses Overflows into InvalidFormat for speed
        var result = TryCoerce("99999999999999999999", typeof(int));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.InvalidFormat)
        );
    }

    [Fact]
    public void TryCoerce_IntOverflow_PreciseParsing_ReturnsOverflow()
    {
        var options = new TypeCoercionOptions { UseFastNumericParsing = false };
        var result = TryCoerce("99999999999999999999", typeof(int), options);

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.Overflow) // Using Convert.ChangeType
        );
    }

    // ── Bool coercion (I5) ─────────────────────────────────────────────

    [Theory]
    [InlineData("true", true)]
    [InlineData("True", true)]
    [InlineData("TRUE", true)]
    [InlineData("false", false)]
    [InlineData("False", false)]
    public void TryCoerce_StringToBool_Parses(string input, bool expected)
    {
        var result = TryCoerce(input, typeof(bool));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe(expected)
        );
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(0, false)]
    public void TryCoerce_NumericToBool_Converts(int input, bool expected)
    {
        var result = TryCoerce(input, typeof(bool));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe(expected)
        );
    }

    [Fact]
    public void TryCoerce_InvalidStringToBool_Fails()
    {
        var result = TryCoerce("not-a-bool", typeof(bool));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.InvalidFormat)
        );
    }

    // ── String coercion ────────────────────────────────────────────────

    [Fact]
    public void TryCoerce_IntToString_ReturnsToString()
    {
        var result = TryCoerce(42, typeof(string));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe("42")
        );
    }

    [Fact]
    public void TryCoerce_DateTimeToString_ReturnsToString()
    {
        var dt = new DateTime(2026, 3, 15, 10, 30, 0);
        var result = TryCoerce(dt, typeof(string));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBeOfType<string>(),
            () => ((string)result.Value!).ShouldContain("2026")
        );
    }

    // ── DateTime coercion ──────────────────────────────────────────────

    [Fact]
    public void Coerce_StringToDateOnly_Parses()
    {
        Coerce("2026-03-15", typeof(DateOnly))
            .ShouldBe(new DateOnly(2026, 3, 15));
    }

    [Fact]
    public void Coerce_StringToDateTimeOffset_Parses()
    {
        var result = Coerce("2026-03-15T10:30:00-05:00", typeof(DateTimeOffset));
        result.ShouldBeOfType<DateTimeOffset>();
    }

    [Fact]
    public void Coerce_DateOnlyToDateTime_Converts()
    {
        var result = Coerce(new DateOnly(2026, 3, 15), typeof(DateTime));
        result.ShouldSatisfyAllConditions(
            () => result.ShouldBeOfType<DateTime>(),
            () => ((DateTime)result!).Date.ShouldBe(new DateTime(2026, 3, 15))
        );
    }

    [Fact]
    public void TryCoerce_DateTimeToDateOnly_Converts()
    {
        var result = TryCoerce(new DateTime(2026, 3, 15, 10, 30, 0), typeof(DateOnly));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe(new DateOnly(2026, 3, 15))
        );
    }

    [Fact]
    public void TryCoerce_DateTimeOffsetToDateTime_Converts()
    {
        var dto = new DateTimeOffset(2026, 3, 15, 10, 30, 0, TimeSpan.FromHours(-5));
        var result = TryCoerce(dto, typeof(DateTime));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBeOfType<DateTime>()
        );
    }

    [Fact]
    public void TryCoerce_DateTimeToDateTimeOffset_Converts()
    {
        var dt = new DateTime(2026, 3, 15, 10, 30, 0, DateTimeKind.Utc);
        var result = TryCoerce(dt, typeof(DateTimeOffset));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBeOfType<DateTimeOffset>()
        );
    }

    // ── Guid coercion ──────────────────────────────────────────────────

    [Fact]
    public void Coerce_StringToGuid_Parses()
    {
        var guid = Guid.NewGuid();
        Coerce(guid.ToString(), typeof(Guid)).ShouldBe(guid);
    }

    // ── Time coercion ──────────────────────────────────────────────────

    [Fact]
    public void Coerce_StringToTimeOnly_Parses()
    {
        Coerce("14:30:00", typeof(TimeOnly))
            .ShouldBe(new TimeOnly(14, 30, 0));
    }

    [Fact]
    public void Coerce_StringToTimeSpan_Parses()
    {
        Coerce("01:02:03", typeof(TimeSpan))
            .ShouldBe(new TimeSpan(1, 2, 3));
    }

    // ── Enum coercion ──────────────────────────────────────────────────

    [Theory]
    [InlineData("tuesday", DayOfWeek.Tuesday)]
    [InlineData("FRIDAY", DayOfWeek.Friday)]
    [InlineData("Monday", DayOfWeek.Monday)]
    public void Coerce_StringToEnum_CaseInsensitive(string input, DayOfWeek expected)
    {
        Coerce(input, typeof(DayOfWeek)).ShouldBe(expected);
    }

    [Fact]
    public void TryCoerce_EnumNumericString_ReturnsInvalidEnumMember()
    {
        var result = TryCoerce("1", typeof(DayOfWeek));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.InvalidEnumMember),
            () => result.Error.ShouldContain("Enum values must be string names only")
        );
    }

    [Fact]
    public void TryCoerce_EnumNumericValue_ReturnsUnsupportedSourceType()
    {
        var result = TryCoerce(1, typeof(DayOfWeek));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.UnsupportedSourceType),
            () => result.Error.ShouldContain("Enum values must be string names only")
        );
    }

    [Fact]
    public void TryCoerce_InvalidEnumName_ReturnsInvalidEnumMember()
    {
        var result = TryCoerce("NotADay", typeof(DayOfWeek));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.InvalidEnumMember)
        );
    }

    // ── Fallback coercion ──────────────────────────────────────────────

    [Fact]
    public void TryCoerce_UnsupportedTypeViaFallback_UsesConvertChangeType()
    {
        // char is not registered in the coercer dictionary — falls through to FallbackTypeCoercer
        var result = TryCoerce("A", typeof(char));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe('A')
        );
    }

    [Fact]
    public void TryCoerce_IncompatibleFallback_ReturnsConversionFailed()
    {
        var result = TryCoerce(new object(), typeof(int));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.ConversionFailed)
        );
    }

    // ── JsonElement coercion ───────────────────────────────────────────

    [Fact]
    public void TryCoerce_JsonElementStringToInt_Succeeds()
    {
        var value = ParseJsonElement("\"123\"");

        var result = TryCoerce(value, typeof(int));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe(123)
        );
    }

    [Fact]
    public void TryCoerce_JsonElementNumberToDecimal_Succeeds()
    {
        var value = ParseJsonElement("123.45");

        var result = TryCoerce(value, typeof(decimal));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe(123.45m)
        );
    }

    [Theory]
    [InlineData("42", typeof(byte), (byte)42)]
    [InlineData("1000", typeof(short), (short)1000)]
    [InlineData("50000", typeof(ushort), (ushort)50000)]
    [InlineData("100000", typeof(uint), (uint)100000)]
    [InlineData("9999999999", typeof(ulong), (ulong)9999999999)]
    public void TryCoerce_JsonElementNumberToSmallIntTypes_Succeeds(string json, Type targetType, object expected)
    {
        var value = ParseJsonElement(json);
        var result = TryCoerce(value, targetType);

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe(expected)
        );
    }

    [Fact]
    public void TryCoerce_JsonElementTrueToBool_Succeeds()
    {
        var value = ParseJsonElement("true");

        var result = TryCoerce(value, typeof(bool));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe(true)
        );
    }

    [Fact]
    public void TryCoerce_JsonElementNullToNullableInt_Succeeds()
    {
        var value = ParseJsonElement("null");

        var result = TryCoerce(value, typeof(int?));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBeNull()
        );
    }

    [Fact]
    public void TryCoerce_JsonElementNullToNonNullableInt_Fails()
    {
        var value = ParseJsonElement("null");

        var result = TryCoerce(value, typeof(int));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.ConversionFailed)
        );
    }

    [Fact]
    public void TryCoerce_JsonElementObjectToComplexType_Deserializes()
    {
        var value = ParseJsonElement("""{"name":"Ada","age":42}""");

        var result = TryCoerce(value, typeof(JsonPayload));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBeOfType<JsonPayload>(),
            () => ((JsonPayload)result.Value!).Name.ShouldBe("Ada"),
            () => ((JsonPayload)result.Value!).Age.ShouldBe(42)
        );
    }

    [Fact]
    public void TryCoerce_JsonElementArrayToListType_Deserializes()
    {
        var value = ParseJsonElement("[1,2,3]");

        var result = TryCoerce(value, typeof(List<int>));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBeOfType<List<int>>(),
            () => ((List<int>)result.Value!).ShouldBe([1, 2, 3])
        );
    }

    [Fact]
    public void TryCoerce_JsonElementObjectToPrimitiveType_FailsExplicitly()
    {
        var value = ParseJsonElement("""{"value":123}""");

        var result = TryCoerce(value, typeof(int));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.UnsupportedSourceType)
        );
    }

    [Fact]
    public void TryCoerce_JsonElementArrayToString_FailsExplicitly()
    {
        var value = ParseJsonElement("""["a","b"]""");

        var result = TryCoerce(value, typeof(string));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.UnsupportedSourceType)
        );
    }

    // ── Coerce exception behavior ──────────────────────────────────────

    [Fact]
    public void Coerce_InvalidConversion_ThrowsWithErrorCode()
    {
        var exception = Should.Throw<TypeCoercionException>(() =>
            Coerce("not-a-guid", typeof(Guid)));

        exception.ErrorCode.ShouldBe(CoercionErrorCode.InvalidFormat);
    }

    [Fact]
    public void Coerce_InvalidConversion_PreservesInnerException()
    {
        var exception = Should.Throw<TypeCoercionException>(() =>
            Coerce(new object(), typeof(int)));

        exception.ShouldSatisfyAllConditions(
            () => exception.InnerException.ShouldNotBeNull(),
            () => exception.InnerException.ShouldBeOfType<InvalidCastException>()
        );
    }

    // ── CoercionResult invariants ──────────────────────────────────────

    [Fact]
    public void CoercionResult_Ok_HasExpectedDefaults()
    {
        var result = CoercionResult.Ok(42);

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe(42),
            () => result.Error.ShouldBe(string.Empty),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.None),
            () => result.OriginalException.ShouldBeNull()
        );
    }

    [Fact]
    public void CoercionResult_Fail_HasExpectedDefaults()
    {
        var result = CoercionResult.Fail("error msg", CoercionErrorCode.InvalidFormat);

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.Value.ShouldBeNull(),
            () => result.Error.ShouldBe("error msg"),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.InvalidFormat),
            () => result.OriginalException.ShouldBeNull()
        );
    }

    [Fact]
    public void CoercionResult_FailWithException_PreservesException()
    {
        var ex = new FormatException("bad format");
        var result = CoercionResult.Fail("error msg", CoercionErrorCode.InvalidFormat, ex);

        result.OriginalException.ShouldBe(ex);
    }

    [Fact]
    public void CoercionResult_Default_RepresentsFailure()
    {
        var result = default(CoercionResult);

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.Value.ShouldBeNull(),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.None)
        );
    }

    // ── TryCoerce never throws (C1 contract) ──────────────────────────

    [Fact]
    public void TryCoerce_NeverThrows_ForAnyInput()
    {
        // This tests that the exception filter removal (C1) works correctly.
        // Passing an unusual type that might trigger unexpected exceptions.
        var result = TryCoerce(new object(), typeof(DateTime));

        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.ErrorCode.ShouldNotBe(CoercionErrorCode.None)
        );
    }

    private static JsonElement ParseJsonElement(string json)
    {
        using var document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }
}
