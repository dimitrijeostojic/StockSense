using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace StockSense.API.Middlewares;

public sealed class GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
    : IMiddleware
{
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var errors = ex.Errors
                .Select(e => new { property = e.PropertyName, message = e.ErrorMessage })
                .ToList();

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                status = 400,
                title = "Validation failed",
                errors
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new ProblemDetails
            {
                Status = 500,
                Title = "Server error",
                Detail = "An internal server error has occurred."
            }));
        }
    }
}
