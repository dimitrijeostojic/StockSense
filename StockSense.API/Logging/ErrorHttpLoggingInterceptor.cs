using Microsoft.AspNetCore.HttpLogging;

namespace StockSense.API.Logging;

public sealed class ErrorHttpLoggingInterceptor : IHttpLoggingInterceptor
{
    public ValueTask OnRequestAsync(HttpLoggingInterceptorContext logContext)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask OnResponseAsync(HttpLoggingInterceptorContext logContext)
    {
        if (logContext.HttpContext.Response.StatusCode >= 400)
        {
            logContext.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders
                | HttpLoggingFields.RequestQuery
                | HttpLoggingFields.ResponsePropertiesAndHeaders
                | HttpLoggingFields.Duration
                | HttpLoggingFields.ResponseBody;
        }
        else
        {
            logContext.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders
                | HttpLoggingFields.RequestQuery
                | HttpLoggingFields.ResponsePropertiesAndHeaders
                | HttpLoggingFields.Duration;
        }

        return ValueTask.CompletedTask;
    }
}