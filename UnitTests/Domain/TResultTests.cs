using Domain.Core;
using FluentAssertions;

namespace UnitTests.Domain;

public sealed class TResultTests
{
    private sealed class Dummy { public string Value { get; init; } = string.Empty; }

    [Fact]
    public void Success_WithValue_IsSuccessTrue()
    {
        var value = new Dummy { Value = "hello" };

        var result = TResult<Dummy>.Success(value);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Success_WithValue_ContainsValue()
    {
        var value = new Dummy { Value = "hello" };

        var result = TResult<Dummy>.Success(value);

        result.Value.Should().BeSameAs(value);
    }

    [Fact]
    public void Success_WithNullValue_IsSuccessTrue()
    {
        var result = TResult<Dummy>.Success(null);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Fact]
    public void Failure_WithError_IsSuccessFalse()
    {
        var error = new Error("TResult.Error", "desc");

        var result = TResult<Dummy>.Failure(error);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Failure_WithError_ValueIsNull()
    {
        var error = new Error("TResult.Error", "desc");

        var result = TResult<Dummy>.Failure(error);

        result.Value.Should().BeNull();
    }

    [Fact]
    public void Failure_WithError_ErrorIsSet()
    {
        var error = new Error("TResult.Error", "desc");

        var result = TResult<Dummy>.Failure(error);

        result.Error.Should().Be(error);
    }
}
