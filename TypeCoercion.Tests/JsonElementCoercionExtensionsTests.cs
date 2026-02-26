using System.Text.Json;
using Shouldly;
using TypeCoercion.Extensions;
using Xunit;

namespace TypeCoercion.Tests;

public class JsonElementCoercionExtensionsTests
{
    [Fact]
    public void CoerceTo_Works()
    {
        var json = JsonDocument.Parse("42").RootElement;
        json.CoerceTo<int>().ShouldBe(42);
    }
}
