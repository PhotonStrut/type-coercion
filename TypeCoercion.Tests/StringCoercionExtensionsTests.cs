using FluentAssertions;
using QueryBuilder.Core.Coercion.Extensions;
using Xunit;

namespace QueryBuilder.Core.Tests.Coercion;

public class StringCoercionExtensionsTests
{
    [Fact]
    public void CoerceTo_Works()
    {
        "42".CoerceTo<int>().Should().Be(42);
    }
    
    [Fact]
    public void TryCoerceTo_WithOutParam_Works()
    {
        var success = "42".TryCoerceTo<int>(out var result);
        success.Should().BeTrue();
        result.Should().Be(42);
        
        var fail = "xyz".TryCoerceTo<int>(out var failResult);
        fail.Should().BeFalse();
        failResult.Should().Be(0);
    }
}
