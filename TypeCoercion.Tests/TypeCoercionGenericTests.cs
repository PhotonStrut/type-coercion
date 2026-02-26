using FluentAssertions;
using QueryBuilder.Core.Coercion;
using Xunit;

namespace QueryBuilder.Core.Tests.Coercion;

public class TypeCoercionGenericTests
{
    [Fact]
    public void TryCoerceGeneric_Success_ReturnsTypedResult()
    {
        var result = QueryBuilder.Core.Coercion.TypeCoercion.TryCoerce<int>("42");
        
        result.Success.Should().BeTrue();
        result.Value.Should().Be(42);
    }
    
    [Fact]
    public void TryCoerceGeneric_Failure_ReturnsFailedTypedResult()
    {
        var result = QueryBuilder.Core.Coercion.TypeCoercion.TryCoerce<int>("not-a-number");
        
        result.Success.Should().BeFalse();
        result.Value.Should().Be(0);
    }

    [Fact]
    public void CoerceGeneric_Success_ReturnsTypedValue()
    {
        var value = QueryBuilder.Core.Coercion.TypeCoercion.Coerce<int>("42");
        value.Should().Be(42);
    }
}
