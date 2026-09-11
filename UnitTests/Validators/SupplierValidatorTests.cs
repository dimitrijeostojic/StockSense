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
        var request = new CreateSupplierRequest("Acme", "John", "john@acme.com", "SUP-001", "123456", null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithEmptyName_HasErrorForName(string? name)
    {
        var request = new CreateSupplierRequest(name!, "John", "john@acme.com", "SUP-001", "000", null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Supplier name is required.");
    }

    [Fact]
    public void Validate_WithNameExceeding100Characters_HasErrorForName()
    {
        var request = new CreateSupplierRequest(new string('A', 101), "John", "john@acme.com", "SUP-001", "000", null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Supplier name must not exceed 100 characters.");
    }

    [Fact]
    public void Validate_WithContactNameExceeding255Characters_HasError()
    {
        var request = new CreateSupplierRequest("Acme", new string('B', 256), "john@acme.com", "SUP-001", "000", null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ContactName);
    }

    [Fact]
    public void Validate_WithContactEmailExceeding255Characters_HasError()
    {
        var request = new CreateSupplierRequest("Acme", "John", new string('C', 256), "SUP-001", "000", null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ContactEmail);
    }

    [Fact]
    public void Validate_WithContactPhoneExceeding20Characters_HasError()
    {
        var request = new CreateSupplierRequest("Acme", "John", "john@acme.com", "SUP-001", new string('9', 21), null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ContactPhone)
              .WithErrorMessage("Supplier contact phone must not exceed 20 characters.");
    }

    [Fact]
    public void Validate_WithContactPhoneExactly20Characters_HasNoError()
    {
        var request = new CreateSupplierRequest("Acme", "John", "john@acme.com", "SUP-001", new string('9', 20), null, null, null);

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
        var request = new UpdateSupplierRequest(Guid.NewGuid(), "Acme", "John", "j@j.com", "SUP-001", "000", null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptySupplierPublicId_HasError()
    {
        var request = new UpdateSupplierRequest(Guid.Empty, "Acme", "John", "j@j.com", "SUP-001", null, null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.SupplierPublicId)
              .WithErrorMessage("SupplierPublicId is required.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Validate_WithEmptyName_HasError(string? name)
    {
        var request = new UpdateSupplierRequest(Guid.NewGuid(), name!, "John", "j@j.com", "SUP-001", null, null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNameExceeding100Characters_HasError()
    {
        var request = new UpdateSupplierRequest(Guid.NewGuid(), new string('A', 101), "John", "j@j.com", "SUP-001", null, null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithContactPhoneExceeding20Characters_HasError()
    {
        var request = new UpdateSupplierRequest(Guid.NewGuid(), "Acme", "John", "j@j.com", "SUP-001", new string('1', 21), null, null, null);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ContactPhone);
    }
}
