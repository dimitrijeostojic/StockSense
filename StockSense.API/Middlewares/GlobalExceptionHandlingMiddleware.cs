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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            var exceptionDetails = GetExceptionDetails(ex);
            context.Response.StatusCode = exceptionDetails.Status;
            context.Response.ContentType = "application/json";
            var problemDetails = new ProblemDetails
            {
                Status = exceptionDetails.Status,
                Type = exceptionDetails.Type,
                Title = exceptionDetails.Title,
                Detail = exceptionDetails.Detail,
            };
            if (exceptionDetails.Errors.Any())
            {
                problemDetails.Extensions["errors"] = exceptionDetails.Errors;
            }
            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
        }
    }

    private static ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => new ExceptionDetails()
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "ValidationFailure",
                Title = "Validation failed",
                Detail = "One or more validation errors has occured",
                Errors = validationException.Errors.Select(e => new
                {
                    e.PropertyName,
                    e.ErrorMessage
                })
            },

            _ => new ExceptionDetails()
            {
                Status = StatusCodes.Status500InternalServerError,
                Type = "ServerError",
                Title = "Server error",
                Detail = "An internal server error has occurred."
            }
        };
    }

    internal class ExceptionDetails
    {
        public int Status { get; set; }
        public string? Title { get; set; }
        public string? Type { get; set; }
        public string? Detail { get; set; }
        public IEnumerable<object> Errors { get; set; } = [];
    }
}
