using Domain.Core;

namespace Application.Common.Errors;

public static class CategoryErrors
{
    public static readonly Error NotFound =
      new("CategoryErrors.NotFound", ErrorType.NotFound, "The requested resource was not found.");
}
