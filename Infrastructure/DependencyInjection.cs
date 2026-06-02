using Application.Behaviors;
using Domain.Abstractions;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Options;
using Infrastructure.RepositoryImplementations;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class DependencyInjection
{
    private const string _sectionName = "Redis";
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories();

        services.AddScoped<UpdateAuditableEntitiesInterceptor>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>();

            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                .AddInterceptors(interceptor);
        });
        services.AddDbContext<AuthDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("AuthConnection"),
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
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

        services.AddIdentityCore<ApplicationUser>() //konfiguracija identity servisa
          .AddRoles<IdentityRole>() //dodavanje podrske za role
                                    //.AddTokenProvider<DataProtectorTokenProvider<User>>("") //dodavanje token provajdera
          .AddEntityFrameworkStores<AuthDbContext>() //podesavanje entity framework skladista
          .AddDefaultTokenProviders(); //dodavanje podrazumevanih token provajdera

        services.AddStackExchangeRedisCache(options =>
        {
            var redisOptions = configuration.GetSection(_sectionName).Get<RedisOptions>();
            options.Configuration = redisOptions!.ConnectionString;
        });

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IJwtRepository, JwtRepository>();
        return services;
    }
}
