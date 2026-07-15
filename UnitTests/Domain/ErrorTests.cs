using Domain.Core;
using FluentAssertions;
using Xunit;

namespace UnitTests.Domain;

public sealed class ErrorTests
{
    [Fact]
    public void None_HasEmptyCode()
    {
        Error.None.Code.Should().BeEmpty();
    }

    [Fact]
    public void None_HasNullDescription()
    {
        Error.None.Description.Should().BeNull();
    }

    [Fact]
    public void Create_WithCodeAndDescription_StoresValues()
    {
        var error = new Error("Domain.SomeError", "A useful description");

        error.Code.Should().Be("Domain.SomeError");
        error.Description.Should().Be("A useful description");
    }

    [Fact]
    public void Create_WithCodeOnly_DescriptionIsNull()
    {
        var error = new Error("Domain.SomeError");

        error.Description.Should().BeNull();
    }

    [Fact]
    public void TwoErrors_WithSameCodeAndDescription_AreEqual()
    {
        var a = new Error("X", "Y");
        var b = new Error("X", "Y");

        a.Should().Be(b);
    }

    [Fact]
    public void TwoErrors_WithDifferentCode_AreNotEqual()
    {
        var a = new Error("X", "Y");
        var b = new Error("Z", "Y");

        a.Should().NotBe(b);
    }
}
