using Application;
using Application.Abstractions;
using Infrastructure;
using StockSense.API.Accessors;
using StockSense.API.Extensions;
using StockSense.API.Logging;
using StockSense.API.Middlewares;
using StockSense.API.OptionsSetup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpLoggingInterceptor<ErrorHttpLoggingInterceptor>();
builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();
builder.Services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();

builder.Services.ConfigureOptions<RabbitMqOptionsSetup>();
builder.Services.ConfigureOptions<RedisOptionsSetup>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseAuthorization();

await app.ApplyMigrationAsync();

app.MapControllers();

app.Run();
