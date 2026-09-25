using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace UnitTests.Domain;

public sealed class ApplicationUserEntityTests
{
    [Fact]
    public void GetSeenTourPages_WhenSeenTourPagesIsNull_ReturnsEmptyList()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);

        user.GetSeenTourPages().Should().BeEmpty();
    }

    [Fact]
    public void CompleteTour_AddsPageName()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);

        user.CompleteTour("products");

        user.GetSeenTourPages().Should().ContainSingle().Which.Should().Be("products");
    }

    [Fact]
    public void CompleteTour_IsIdempotent()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);

        user.CompleteTour("products");
        user.CompleteTour("products");

        user.GetSeenTourPages().Should().ContainSingle().Which.Should().Be("products");
    }

    [Fact]
    public void CompleteTour_MultiplePages_AddsAll()
    {
        var user = ApplicationUser.Create("john", "john@test.com", "John", "Doe", 1);

        user.CompleteTour("products");
        user.CompleteTour("suppliers");
        user.CompleteTour("orders");

        user.GetSeenTourPages().Should().BeEquivalentTo(["products", "suppliers", "orders"]);
    }
}
