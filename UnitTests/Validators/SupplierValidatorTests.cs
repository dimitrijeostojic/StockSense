using Application.SupplierManagement.CreateSupplier;
using Application.SupplierManagement.UpdateSupplier;
using FluentValidation.TestHelper;
using Xunit;

namespace UnitTests.Validators;

public sealed class CreateSupplierValidatorTests
{
    private readonly CreateSupplierValidator _sut = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var request = new CreateSupplierRequest("Acme", "John", "john@acme.com", "123456");

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithEmptyName_HasErrorForName(string? name)
    {
        var request = new CreateSupplierRequest(name!, "John", null, "000");

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Supplier name is required.");
    }

    [Fact]
    public void Validate_WithNameExceeding100Characters_HasErrorForName()
    {
        var request = new CreateSupplierRequest(new string('A', 101), "John", null, "000");

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Supplier name must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_WithContactNameExceeding255Characters_HasError()
    {
        var request = new CreateSupplierRequest("Acme", new string('B', 256), null, "000");

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ContactName);
    }

    [Fact]
    public void Validate_WithContactEmailExceeding255Characters_HasError()
    {
        var request = new CreateSupplierRequest("Acme", "John", new string('C', 256), "000");

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ContactEmail);
    }

    [Fact]
    public void Validate_WithContactPhoneExceeding20Characters_HasError()
    {
        var request = new CreateSupplierRequest("Acme", "John", null, new string('9', 21));

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ContactPhone)
              .WithErrorMessage("Supplier contact phone must not exceed 20 characters.");
    }

    [Fact]
    public void Validate_WithContactPhoneExactly20Characters_HasNoError()
    {
        var request = new CreateSupplierRequest("Acme", "John", null, new string('9', 20));

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.ContactPhone);
    }
}

public sealed class UpdateSupplierValidatorTests
{
    private readonly UpdateSupplierValidator _sut = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoErrors()
    {
        var request = new UpdateSupplierRequest(Guid.NewGuid(), "Acme", "John", "j@j.com", "000");

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptySupplierPublicId_HasError()
    {
        var request = new UpdateSupplierRequest(Guid.Empty, "Acme", null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SupplierPublicId)
              .WithErrorMessage("SupplierPublicId is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithEmptyName_HasError(string? name)
    {
        var request = new UpdateSupplierRequest(Guid.NewGuid(), name!, null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNameExceeding100Characters_HasError()
    {
        var request = new UpdateSupplierRequest(Guid.NewGuid(), new string('A', 101), null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithContactPhoneExceeding20Characters_HasError()
    {
        var request = new UpdateSupplierRequest(Guid.NewGuid(), "Acme", null, null, new string('1', 21));

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ContactPhone);
    }
}
