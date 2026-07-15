using Application.CategoryManagement.CreateCategory;
using FluentValidation.TestHelper;
using Xunit;

namespace UnitTests.Validators;

public sealed class CreateCategoryValidatorTests
{
    private readonly CreateCategoryValidator _sut = new();

    [Fact]
    public void Validate_WithValidRequest_HasNoValidationErrors()
    {
        var request = new CreateCategoryRequest("Electronics", "All electronics");

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithNullDescription_HasNoValidationErrors()
    {
        var request = new CreateCategoryRequest("Books", null!);

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyOrNullName_HasValidationErrorForName(string? name)
    {
        var request = new CreateCategoryRequest(name!, "desc");

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Validate_WithNameExceeding255Characters_HasValidationErrorForName()
    {
        var longName = new string('A', 256);
        var request = new CreateCategoryRequest(longName, "desc");

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Name)
              .WithErrorMessage("Name must not exceed 255 characters.");
    }

    [Fact]
    public void Validate_WithDescriptionExceeding255Characters_HasValidationErrorForDescription()
    {
        var longDesc = new string('B', 256);
        var request = new CreateCategoryRequest("Valid", longDesc);

        var result = _sut.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Description)
              .WithErrorMessage("Description must not exceed 255 characters.");
    }

    [Fact]
    public void Validate_WithDescriptionExactly255Characters_HasNoValidationErrors()
    {
        var maxDesc = new string('C', 255);
        var request = new CreateCategoryRequest("Valid", maxDesc);

        var result = _sut.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }
}
