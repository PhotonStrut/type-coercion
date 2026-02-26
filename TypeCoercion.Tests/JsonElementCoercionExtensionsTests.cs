using System.Text.Json;
using FluentAssertions;
using QueryBuilder.Core.Coercion.Extensions;
using Xunit;

namespace QueryBuilder.Core.Tests.Coercion;

public class JsonElementCoercionExtensionsTests
{
    [Fact]
    public void CoerceTo_Works()
    {
        var json = JsonDocument.Parse("42").RootElement;
        json.CoerceTo<int>().Should().Be(42);
    }
}
