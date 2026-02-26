using System.Collections.Generic;
using FluentAssertions;
using QueryBuilder.Core.Coercion.Extensions;
using Xunit;

namespace QueryBuilder.Core.Tests.Coercion;

public class DictionaryCoercionExtensionsTests
{
    [Fact]
    public void CoerceValue_Works()
    {
        var dict = new Dictionary<string, object?> { { "Age", "42" } };
        dict.CoerceValue<int>("Age").Should().Be(42);
    }
    
    [Fact]
    public void CoerceValueOrDefault_MissingKey_ReturnsDefault()
    {
        var dict = new Dictionary<string, object?>();
        dict.CoerceValueOrDefault<int>("Age", 99).Should().Be(99);
    }
}
