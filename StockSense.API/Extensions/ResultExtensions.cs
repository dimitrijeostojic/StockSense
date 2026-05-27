using Domain.Core;
using Microsoft.AspNetCore.Mvc;

namespace StockSense.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this Result<T> result) where T : class
    {
        if (!result.IsSuccess)
        {
            if (result.Error?.Type == ErrorType.NotFound)
            {
                return new NotFoundObjectResult(result.Error);
            }
            return new BadRequestObjectResult(result.Error);
        }
        else
        {
            return new OkObjectResult(result.Value);

        }
    }
}
