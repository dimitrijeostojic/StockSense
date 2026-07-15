using Application.OrderManagement.CreateOrder;
using Application.OrderManagement.UpdateOrder;
using FluentValidation.TestHelper;
using Xunit;

namespace UnitTests.Validators;

public sealed class CreateOrderValidatorTests
{
    private readonly CreateOrderValidator _sut = new();

    private static CreateOrderRequest ValidRequest() => new(
        Guid.NewGuid(),
        DateTime.UtcNow.AddDays(-1),
        null,
        [new OrderItemDto(Guid.NewGuid(), 2)]);

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var result = _sut.TestValidate(ValidRequest());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptySupplierPublicId_HasError()
    {
        var request = ValidRequest() with { SupplierPublicId = Guid.Empty };

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SupplierPublicId);
    }

    [Fact]
    public void Validate_WithEmptyOrderItemsCollection_HasError()
    {
        var request = ValidRequest() with { OrderItemsDto = [] };

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.OrderItemsDto)
              .WithErrorMessage("At least one order item is required.");
    }

    [Fact]
    public void Validate_WithNotesExceeding255Characters_HasError()
    {
        var request = ValidRequest() with { Notes = new string('N', 256) };

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Notes);
    }

    [Fact]
    public void Validate_WithNotesExactly255Characters_HasNoError()
    {
        var request = ValidRequest() with { Notes = new string('N', 255) };

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Notes);
    }
}

public sealed class UpdateOrderValidatorTests
{
    private readonly UpdateOrderValidator _sut = new();

    private static UpdateOrderRequest ValidRequest() => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        DateTime.UtcNow.AddDays(-1),
        null);

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var result = _sut.TestValidate(ValidRequest());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyPublicId_HasError()
    {
        var request = ValidRequest() with { PublicId = Guid.Empty };

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PublicId);
    }

    [Fact]
    public void Validate_WithEmptySupplierPublicId_HasError()
    {
        var request = ValidRequest() with { SupplierPublicId = Guid.Empty };

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SupplierPublicId);
    }

    [Fact]
    public void Validate_WithFutureOrderDate_HasError()
    {
        var request = ValidRequest() with { OrderDate = DateTime.UtcNow.AddDays(1) };

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.OrderDate)
              .WithErrorMessage("OrderDate cannot be in the future.");
    }

    [Fact]
    public void Validate_WithNotesExceeding500Characters_HasError()
    {
        var request = ValidRequest() with { Notes = new string('Z', 501) };

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Notes)
              .WithErrorMessage("Notes cannot exceed 500 characters.");
    }

    [Fact]
    public void Validate_WithNotesExactly500Characters_HasNoError()
    {
        var request = ValidRequest() with { Notes = new string('Z', 500) };

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Notes);
    }
}
