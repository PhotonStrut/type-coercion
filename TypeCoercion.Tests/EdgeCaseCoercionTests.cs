using System;
using System.Collections.Generic;
using AutoFixture;
using NSubstitute;
using QueryBuilder.Core.Coercion;
using QueryBuilder.Core.Coercion.Coercers;
using Shouldly;
using Xunit;
using static QueryBuilder.Core.Coercion.TypeCoercion;

namespace QueryBuilder.Core.Tests.Coercion;

public class EdgeCaseCoercionTests
{
    private readonly IFixture _fixture;

    public EdgeCaseCoercionTests()
    {
        _fixture = new Fixture();
    }

    [Theory]
    [ClassData(typeof(NumericEdgeCaseProvider))]
    public void NumericCoercer_EdgeCases_HandleCorrectly(object input, Type targetType, bool shouldSucceed)
    {
        var result = TryCoerce(input, targetType);

        if (shouldSucceed)
        {
            result.Success.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
        }
        else
        {
            result.Success.ShouldBeFalse();
            // Edge cases usually fail due to overflow or format issues
            result.ErrorCode.ShouldBeOneOf(CoercionErrorCode.Overflow, CoercionErrorCode.InvalidFormat, CoercionErrorCode.UnsupportedSourceType, CoercionErrorCode.ConversionFailed);
        }
    }

    [Fact]
    public void TryCoerce_WithCustomMockedCoercer_DispatchesCorrectly()
    {
        // 1. Setup NSubstitute
        var mockCoercer = Substitute.For<ITypeCoercer>();
        var targetType = typeof(DateTime);
        var input = "MockDate";
        
        // 2. Setup mock behavior
        mockCoercer.TryCoerce(input, targetType, targetType, Arg.Any<TypeCoercionOptions>())
                   .Returns(CoercionResult.Ok(new DateTime(2099, 1, 1)));

        var options = new TypeCoercionOptions();
        options.Coercers.Insert(0, mockCoercer);

        // 3. Execute
        var result = TryCoerce(input, targetType, options);

        // 4. Assert with Shouldly constraints
        result.Success.ShouldBeTrue();
        result.Value.ShouldBe(new DateTime(2099, 1, 1));
        
        // Verify mock was called
        mockCoercer.Received(1).TryCoerce(input, targetType, targetType, options);
    }
    
    [Fact]
    public void AutoFixture_GenericProperties_ShouldCoerceToSameTypes()
    {
        // Generate random guids, complex strings with AutoFixture
        var randomGuidStr = _fixture.Create<Guid>().ToString();
        var randomIntStr = _fixture.Create<int>().ToString();
        var randomDecimal = _fixture.Create<decimal>();
        
        TryCoerce(randomGuidStr, typeof(Guid)).Success.ShouldBeTrue();
        TryCoerce(randomIntStr, typeof(int)).Success.ShouldBeTrue();
        
        var decimalCoerce = TryCoerce(randomDecimal, typeof(decimal));
        decimalCoerce.Success.ShouldBeTrue();
        decimalCoerce.Value.ShouldBe(randomDecimal);
    }

    private class NumericEdgeCaseProvider : TheoryData<object, Type, bool>
    {
        public NumericEdgeCaseProvider()
        {
            // Max Values
            Add(int.MaxValue.ToString(), typeof(int), true);
            Add(long.MaxValue.ToString(), typeof(long), true);
            Add(double.MaxValue.ToString("R"), typeof(double), true);

            // Overflow Cases
            Add(long.MaxValue.ToString(), typeof(int), false); // long max doesn't fit in int
            Add(int.MinValue.ToString(), typeof(uint), false); // negatives don't fit in uint
            Add("1.7976931348623157E+308", typeof(float), true); // double max parsed as float resolves to Infinity, but parsing succeeds
            
            // Precision loss checks
            Add("12.34567890123456789", typeof(decimal), true);
        }
    }
}
