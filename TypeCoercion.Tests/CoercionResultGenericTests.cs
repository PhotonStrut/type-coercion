using Shouldly;
using TypeCoercion;
using Xunit;

namespace TypeCoercion.Tests;

public class CoercionResultGenericTests
{
    [Fact]
    public void Ok_ReturnsSuccessfulResult()
    {
        var result = CoercionResult<int>.Ok(42);
        
        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe(42),
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.None),
            () => result.Error.ShouldBeEmpty()
        );
    }
    
    [Fact]
    public void Fail_ReturnsFailedResult()
    {
        var result = CoercionResult<int>.Fail("Oops", CoercionErrorCode.ConversionFailed);
        
        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeFalse(),
            () => result.Value.ShouldBe(0), // default
            () => result.ErrorCode.ShouldBe(CoercionErrorCode.ConversionFailed),
            () => result.Error.ShouldBe("Oops")
        );
    }
    
    [Fact]
    public void ImplicitConversion_ToNonGeneric_Works()
    {
        var generic = CoercionResult<int>.Ok(42);
        CoercionResult nonGeneric = generic;
        
        nonGeneric.ShouldSatisfyAllConditions(
            () => nonGeneric.Success.ShouldBeTrue(),
            () => nonGeneric.Value.ShouldBe(42)
        );
    }
}
