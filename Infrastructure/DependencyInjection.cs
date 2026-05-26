using Application.Behaviors;
using Domain.Abstractions;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Options;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<UpdateAuditableEntitiesInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>();

            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                .AddInterceptors(interceptor);
        });

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                cfg.Host(options.Host, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddStackExchangeRedisCache(options =>
        {
            var redisOptions = services.BuildServiceProvider().GetRequiredService<IOptions<RedisOptions>>().Value;
            options.Configuration = redisOptions.ConnectionString;
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
        return services;
    }
}