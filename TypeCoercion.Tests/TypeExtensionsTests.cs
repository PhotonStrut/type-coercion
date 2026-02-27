using Shouldly;
using TypeCoercion.Extensions;
using Xunit;

namespace TypeCoercion.Tests;

public class TypeExtensionsTests
{
    [Fact]
    public void TryCoerce_Works()
    {
        var result = typeof(int).TryCoerce("42");
        result.ShouldSatisfyAllConditions(
            () => result.Success.ShouldBeTrue(),
            () => result.Value.ShouldBe(42)
        );
    }

    [Fact]
    public void Coerce_Works()
    {
        typeof(int).Coerce("42").ShouldBe(42);
    }

    [Fact]
    public void CoerceOrDefault_Failure_ReturnsTypeDefault()
    {
        typeof(int).CoerceOrDefault("bad").ShouldBe(0);
    }

    [Fact]
    public void CoerceOrNull_Failure_ReturnsNull()
    {
        typeof(Guid).CoerceOrNull("bad").ShouldBeNull();
    }
}
