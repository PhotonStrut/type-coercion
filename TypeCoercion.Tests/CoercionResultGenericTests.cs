using FluentAssertions;
using QueryBuilder.Core.Coercion;
using Xunit;

namespace QueryBuilder.Core.Tests.Coercion;

public class CoercionResultGenericTests
{
    [Fact]
    public void Ok_ReturnsSuccessfulResult()
    {
        var result = CoercionResult<int>.Ok(42);
        
        result.Success.Should().BeTrue();
        result.Value.Should().Be(42);
        result.ErrorCode.Should().Be(CoercionErrorCode.None);
        result.Error.Should().BeEmpty();
    }
    
    [Fact]
    public void Fail_ReturnsFailedResult()
    {
        var result = CoercionResult<int>.Fail("Oops", CoercionErrorCode.ConversionFailed);
        
        result.Success.Should().BeFalse();
        result.Value.Should().Be(0); // default
        result.ErrorCode.Should().Be(CoercionErrorCode.ConversionFailed);
        result.Error.Should().Be("Oops");
    }
    
    [Fact]
    public void ImplicitConversion_ToNonGeneric_Works()
    {
        var generic = CoercionResult<int>.Ok(42);
        CoercionResult nonGeneric = generic;
        
        nonGeneric.Success.Should().BeTrue();
        nonGeneric.Value.Should().Be(42);
    }
}
