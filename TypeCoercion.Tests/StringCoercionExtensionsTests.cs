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
}
