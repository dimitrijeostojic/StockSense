using Application.Behaviors;
using Domain.Abstractions;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>));
        return services;
    }
}