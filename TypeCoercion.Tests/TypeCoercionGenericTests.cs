using Shouldly;
using TypeCoercion;
using static TypeCoercion.TypeCoercion;
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
}
