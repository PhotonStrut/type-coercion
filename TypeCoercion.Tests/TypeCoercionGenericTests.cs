using Shouldly;
using QueryBuilder.Core.Coercion;
using static QueryBuilder.Core.Coercion.TypeCoercion;
using Xunit;

namespace QueryBuilder.Core.Tests.Coercion;

public class TypeCoercionGenericTests
{
    [Fact]
    public void TryCoerceGeneric_Success_ReturnsTypedResult()
    {
        var result = TryCoerce<int>("42");
        
        result.Success.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }
    
    [Fact]
    public void TryCoerceGeneric_Failure_ReturnsFailedTypedResult()
    {
        var result = TryCoerce<int>("not-a-number");
        
        result.Success.ShouldBeFalse();
        result.Value.ShouldBe(0);
    }

    [Fact]
    public void CoerceGeneric_Success_ReturnsTypedValue()
    {
        var value = Coerce<int>("42");
        value.ShouldBe(42);
    }
}
