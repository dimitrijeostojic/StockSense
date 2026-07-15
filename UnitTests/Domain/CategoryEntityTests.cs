using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace UnitTests.Domain;

public sealed class CategoryEntityTests
{
    private static readonly Guid TenantId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidInputs_ReturnsCategoryWithExpectedValues()
    {
        var category = Category.Create("Electronics", "All electronics", TenantId);

        category.Name.Should().Be("Electronics");
        category.Description.Should().Be("All electronics");
        category.TenantPublicId.Should().Be(TenantId);
    }

    [Fact]
    public void Create_WithNullDescription_ReturnsCategoryWithNullDescription()
    {
        var category = Category.Create("Books", null, TenantId);

        category.Description.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullName_ThrowsArgumentException()
    {
        var act = () => Category.Create(null!, "desc", TenantId);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithWhitespaceName_ThrowsArgumentException()
    {
        var act = () => Category.Create("   ", "desc", TenantId);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsArgumentException()
    {
        var act = () => Category.Create(string.Empty, "desc", TenantId);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_HasEmptyProductsCollection()
    {
        var category = Category.Create("Gadgets", null, TenantId);

        category.Products.Should().BeEmpty();
    }

    [Fact]
    public void Create_AssignsNewPublicId()
    {
        var category = Category.Create("Gadgets", null, TenantId);

        category.PublicId.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void WithName_UpdatesName()
    {
        var category = Category.Create("Old", null, TenantId);

        category.WithName("New");

        category.Name.Should().Be("New");
    }

    [Fact]
    public void WithName_ReturnsTheSameCategoryInstance()
    {
        var category = Category.Create("Old", null, TenantId);

        var returned = category.WithName("New");

        returned.Should().BeSameAs(category);
    }

    [Fact]
    public void WithName_WithNullName_ThrowsArgumentException()
    {
        var category = Category.Create("Old", null, TenantId);

        var act = () => category.WithName(null!);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithName_WithWhitespaceName_ThrowsArgumentException()
    {
        var category = Category.Create("Old", null, TenantId);

        var act = () => category.WithName("   ");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void WithDescription_UpdatesDescription()
    {
        var category = Category.Create("Books", "old desc", TenantId);

        category.WithDescription("new desc");

        category.Description.Should().Be("new desc");
    }

    [Fact]
    public void WithDescription_WithNull_SetsDescriptionToNull()
    {
        var category = Category.Create("Books", "old desc", TenantId);

        category.WithDescription(null);

        category.Description.Should().BeNull();
    }
}
