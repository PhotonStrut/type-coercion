using System.Collections.Generic;
using Shouldly;
using QueryBuilder.Core.Coercion.Extensions;
using Xunit;

namespace QueryBuilder.Core.Tests.Coercion;

public class DictionaryCoercionExtensionsTests
{
    [Fact]
    public void CoerceValue_Works()
    {
        var dict = new Dictionary<string, object?> { { "Age", "42" } };
        dict.CoerceValue<int>("Age").ShouldBe(42);
    }
    
    [Fact]
    public void CoerceValueOrDefault_MissingKey_ReturnsDefault()
    {
        var dict = new Dictionary<string, object?>();
        dict.CoerceValueOrDefault<int>("Age", 99).ShouldBe(99);
    }
}
