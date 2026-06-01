using Application.Common.Errors;
using Domain.Core;
using Microsoft.AspNetCore.Mvc;

namespace StockSense.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(this TResult<T> result) where T : class
    {
        if (!result.IsSuccess)
        {
            if (result.Error == ApplicationErrors.NotFound)
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

    public static IActionResult ToActionResult(this Result result)
    {
        if (!result.IsSuccess)
        {
            if (result.Error == ApplicationErrors.NotFound)
            {
                return new NotFoundObjectResult(result.Error);
            }
            return new BadRequestObjectResult(result.Error);
        }
        else
        {
            return new NoContentResult();

        }
    }
}
