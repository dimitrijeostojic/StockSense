using Domain.Core;
using FluentAssertions;
using Xunit;

namespace UnitTests.Domain;

public sealed class ResultTests
{
    [Fact]
    public void Success_ReturnsResultWithIsSuccessTrue()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Success_ReturnsResultWithNoneError()
    {
        var result = Result.Success();

        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ReturnsResultWithIsSuccessFalse()
    {
        var error = new Error("Test.Error", "Something went wrong");

        var result = Result.Failure(error);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void Failure_ReturnsResultWithProvidedError()
    {
        var error = new Error("Test.Error", "Something went wrong");

        var result = Result.Failure(error);

        result.Error.Should().Be(error);
    }

    [Fact]
    public void Failure_WithNullDescription_ReturnsResultWithNullDescription()
    {
        var error = new Error("Test.Error");

        var result = Result.Failure(error);

        result.Error.Description.Should().BeNull();
        result.Error.Code.Should().Be("Test.Error");
    }
}
