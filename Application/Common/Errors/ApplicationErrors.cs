using Domain.Core;

namespace Application.Common.Errors;

public static class ApplicationErrors
{
    public static readonly Error NotFound =
      new("ApplicationErrors.NotFound", "The requested resource was not found.");
}
