using Shouldly;
using TypeCoercion.Extensions;
using Xunit;

namespace TypeCoercion.Tests;

public class StringCoercionExtensionsTests
{
    [Fact]
    public void CoerceTo_Works()
    {
        "42".CoerceTo<int>().ShouldBe(42);
    }
    
    [Fact]
    public void TryCoerceTo_WithOutParam_Works()
    {
        var success = "42".TryCoerceTo<int>(out var result);
        success.ShouldSatisfyAllConditions(
            () => success.ShouldBeTrue(),
            () => result.ShouldBe(42)
        );
        
        var fail = "xyz".TryCoerceTo<int>(out var failResult);
        fail.ShouldSatisfyAllConditions(
            () => fail.ShouldBeFalse(),
            () => failResult.ShouldBe(0)
        );
    }

    [Fact]
    public void CoerceToOrDefault_Failure_ReturnsTypeDefault()
    {
        "bad".CoerceToOrDefault<int>().ShouldBe(0);
    }

    [Fact]
    public void CoerceToOrDefault_Success_ReturnsCoercedValue()
    {
        "42".CoerceToOrDefault<int>().ShouldBe(42);
    }

    [Fact]
    public void CoerceToOrNull_Failure_ReturnsNull()
    {
        "bad".CoerceToOrNull<int?>().ShouldBeNull();
    }

    [Fact]
    public void CoerceToOrNull_NonNullableValueType_Failure_ThrowsInvalidOperationException()
    {
        Should.Throw<System.InvalidOperationException>(() => "bad".CoerceToOrNull<int>());
    }

    [Fact]
    public void CoerceToOrFallback_Failure_ReturnsProvidedFallback()
    {
        "bad".CoerceToOrFallback(5).ShouldBe(5);
    }
}
