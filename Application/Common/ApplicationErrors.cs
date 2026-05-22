using Domain.Core;

namespace Application.Common;

public static class ApplicationErrors
{
    public static readonly Error NotFound =
        new("StockSense.NotFound", "The requested resource was not found.");

}