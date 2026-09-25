using Application.Common.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace StockSense.API.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequirePlatformKeyAttribute : Attribute, IAuthorizationFilter
{
    public const string HeaderName = "X-Platform-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var options = context.HttpContext.RequestServices
            .GetRequiredService<IOptions<AppOptions>>().Value;

        if (string.IsNullOrWhiteSpace(options.PlatformApiKey))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var providedKey)
            || providedKey != options.PlatformApiKey)
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
