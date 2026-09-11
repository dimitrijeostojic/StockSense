using Application.ProductManagement.CreateProduct;
using Application.ProductManagement.UpdateProduct;
using Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace UnitTests.Validators;

public sealed class CreateProductValidatorTests
{
    private readonly CreateProductValidator _sut = new();

    [Fact]
    public void Validate_WithAllValidFields_HasNoErrors()
    {
        var request = new CreateProductRequest(
            "Widget", "SKU-001", "A widget", 9.99m, 5,
            UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithEmptyName_HasErrorForName(string? name)
    {
        var request = new CreateProductRequest(
            name!, "SKU-001", null, 1m, 0, UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNameExceeding100Characters_HasErrorForName()
    {
        var longName = new string('X', 101);
        var request = new CreateProductRequest(
            longName, "SKU-001", null, 1m, 0, UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithEmptySku_HasErrorForSku(string? sku)
    {
        var request = new CreateProductRequest(
            "Widget", sku!, null, 1m, 0, UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Sku);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WithPriceZeroOrNegative_HasErrorForPrice(decimal price)
    {
        var request = new CreateProductRequest(
            "Widget", "SKU-001", null, price, 0, UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Price)
              .WithErrorMessage("Price must be greater than zero.");
    }

    [Fact]
    public void Validate_WithNegativeMinimumStockQuantity_HasErrorForMinimumStockQuantity()
    {
        var request = new CreateProductRequest(
            "Widget", "SKU-001", null, 1m, -1, UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.MinimumStockQuantity);
    }

    [Fact]
    public void Validate_WithZeroMinimumStockQuantity_HasNoErrorForMinimumStockQuantity()
    {
        var request = new CreateProductRequest(
            "Widget", "SKU-001", null, 1m, 0, UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.MinimumStockQuantity);
    }

    [Fact]
    public void Validate_WithEmptyCategoryPublicId_HasErrorForCategoryPublicId()
    {
        var request = new CreateProductRequest(
            "Widget", "SKU-001", null, 1m, 0, UnitOfMeasurement.Piece, Guid.Empty, Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.CategoryPublicId);
    }

    [Fact]
    public void Validate_WithEmptySupplierPublicId_HasErrorForSupplierPublicId()
    {
        var request = new CreateProductRequest(
            "Widget", "SKU-001", null, 1m, 0, UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.Empty);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SupplierPublicId);
    }

    [Fact]
    public void Validate_WithDescriptionExceeding255Characters_HasErrorForDescription()
    {
        var longDesc = new string('D', 256);
        var request = new CreateProductRequest(
            "Widget", "SKU-001", longDesc, 1m, 0, UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}

public sealed class UpdateProductValidatorTests
{
    private readonly UpdateProductValidator _sut = new();

    [Fact]
    public void Validate_WithAllValidFields_HasNoErrors()
    {
        var request = new UpdateProductRequest(
            Guid.NewGuid(), "Widget", "SKU-001", "desc", 9.99m, 5,
            UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyProductPublicId_HasError()
    {
        var request = new UpdateProductRequest(
            Guid.Empty, "Widget", "SKU-001", null, 1m, 0, UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ProductPublicId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithEmptyName_HasError(string? name)
    {
        var request = new UpdateProductRequest(
            Guid.NewGuid(), name!, "SKU-001", null, 1m, 0, UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    public void Validate_WithPriceNotGreaterThanZero_HasError(decimal price)
    {
        var request = new UpdateProductRequest(
            Guid.NewGuid(), "Widget", "SKU-001", null, price, 0, UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Fact]
    public void Validate_WithNegativeMinimumStockQuantity_HasError()
    {
        var request = new UpdateProductRequest(
            Guid.NewGuid(), "Widget", "SKU-001", null, 1m, -1, UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.MinimumStockQuantity);
    }

    [Fact]
    public void Validate_WithEmptyCategoryId_HasError()
    {
        var request = new UpdateProductRequest(
            Guid.NewGuid(), "Widget", "SKU-001", null, 1m, 0, UnitOfMeasurement.Piece, Guid.Empty, Guid.NewGuid());

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void Validate_WithEmptySupplierId_HasError()
    {
        var request = new UpdateProductRequest(
            Guid.NewGuid(), "Widget", "SKU-001", null, 1m, 0, UnitOfMeasurement.Piece, Guid.NewGuid(), Guid.Empty);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SupplierId);
    }
}
