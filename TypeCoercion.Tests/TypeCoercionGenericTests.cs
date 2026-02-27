using Shouldly;
using TypeCoercion;
using static TypeCoercion.TypeCoercer;
using Xunit;

namespace TypeCoercion.Tests;

public class TypeCoercionGenericTests
{
    [Fact]
    public void TryCoerceGeneric_Success_ReturnsTypedResult()
    {
        var result = TryCoerce<int>("42");
        
        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe(42)
        );
    }
    
    [Fact]
    public void TryCoerceGeneric_Failure_ReturnsFailedTypedResult()
    {
        var result = TryCoerce<int>("not-a-number");
        
        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.Value.ShouldBe(0)
        );
    }

    [Fact]
    public void CoerceGeneric_Success_ReturnsTypedValue()
    {
        var value = Coerce<int>("42");
        value.ShouldBe(42);
    }

    [Fact]
    public void CoerceOrDefaultGeneric_Failure_ReturnsTypeDefault()
    {
        var value = CoerceOrDefault<int>("not-a-number");
        value.ShouldBe(0);
    }

    [Fact]
    public void CoerceOrNullGeneric_Failure_ReturnsNull()
    {
        var value = CoerceOrNull<int?>("not-a-number");
        value.ShouldBeNull();
    }

    [Fact]
    public void CoerceOrNullGeneric_NonNullableValueType_Failure_ThrowsInvalidOperationException()
    {
        Should.Throw<System.InvalidOperationException>(() => CoerceOrNull<int>("not-a-number"));
    }

    [Fact]
    public void CoerceOrFallbackGeneric_Failure_ReturnsFallback()
    {
        var value = CoerceOrFallback("not-a-number", 7);
        value.ShouldBe(7);
    }
}
