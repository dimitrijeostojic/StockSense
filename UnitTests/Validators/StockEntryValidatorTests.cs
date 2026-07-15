using Application.ProductManagement.CreateStockEntry;
using Domain.Enums;
using FluentValidation.TestHelper;
using Xunit;

namespace UnitTests.Validators;

public sealed class CreateStockEntryValidatorTests
{
    private readonly CreateStockEntryValidator _sut = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var request = new CreateStockEntryRequest(Guid.NewGuid(), 10, "note", StockEntryType.In);

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyProductPublicId_HasError()
    {
        var request = new CreateStockEntryRequest(Guid.Empty, 10, null, StockEntryType.In);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ProductPublicId)
              .WithErrorMessage("ProductPublicId is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithQuantityNotGreaterThanZero_HasError(int quantity)
    {
        var request = new CreateStockEntryRequest(Guid.NewGuid(), quantity, null, StockEntryType.In);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Quantity)
              .WithErrorMessage("Quantity must be greater than 0.");
    }

    [Fact]
    public void Validate_WithQuantityOfOne_HasNoError()
    {
        var request = new CreateStockEntryRequest(Guid.NewGuid(), 1, null, StockEntryType.Out);

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Quantity);
    }

    [Fact]
    public void Validate_WithNotesExceeding255Characters_HasError()
    {
        var request = new CreateStockEntryRequest(Guid.NewGuid(), 5, new string('N', 256), StockEntryType.In);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Notes)
              .WithErrorMessage("Notes cannot exceed 255 characters.");
    }

    [Fact]
    public void Validate_WithNotesExactly255Characters_HasNoError()
    {
        var request = new CreateStockEntryRequest(Guid.NewGuid(), 5, new string('N', 255), StockEntryType.In);

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Notes);
    }

    [Theory]
    [InlineData(StockEntryType.In)]
    [InlineData(StockEntryType.Out)]
    [InlineData(StockEntryType.Adjustment)]
    public void Validate_WithValidStockEntryType_HasNoError(StockEntryType type)
    {
        var request = new CreateStockEntryRequest(Guid.NewGuid(), 5, null, type);

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.StockEntryType);
    }

    [Fact]
    public void Validate_WithInvalidStockEntryType_HasError()
    {
        var request = new CreateStockEntryRequest(Guid.NewGuid(), 5, null, (StockEntryType)99);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.StockEntryType)
              .WithErrorMessage("StockEntryType must be a valid enum value.");
    }
}
