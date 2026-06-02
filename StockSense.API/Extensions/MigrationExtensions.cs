using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;

namespace StockSense.API.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var authDb = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        AsyncRetryPolicy retryPolicy = Policy
          .Handle<Exception>()
          .WaitAndRetryAsync(
              retryCount: 10,
              sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
              onRetry: (exception, timespan, attempt, _) =>
              {
                  logger.LogWarning(
                      "Migration attempt {Attempt} failed. Waiting {Seconds}s. Error: {Error}",
                      attempt, timespan.TotalSeconds, exception.Message);
              });

        await retryPolicy.ExecuteAsync(async () =>
        {
            logger.LogInformation("Applying database migrations...");
            await db.Database.MigrateAsync();
            logger.LogInformation("Migrations applied successfully.");

        });
        await retryPolicy.ExecuteAsync(async () =>
        {
            logger.LogInformation("Applying auth database migrations...");
            await authDb.Database.MigrateAsync();
            logger.LogInformation("Auth migrations applied successfully.");
        });
    }
}
