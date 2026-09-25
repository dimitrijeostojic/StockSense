using Application.Abstractions.Idempotency;
using Application.Abstractions.Services;
using Domain.Abstractions;
using Domain.Dtos;
using Domain.Entities;
using Domain.RepositoryInterfaces;
using Infrastructure.BackgroundJobs;
using Infrastructure.Data;
using Infrastructure.Data.Interceptors;
using Infrastructure.Interceptors;
using Infrastructure.InternalServiceInterfaces;
using Infrastructure.Options;
using Infrastructure.Pdf;
using Infrastructure.RepositoryImplementations;
using Infrastructure.RepositoryImplementations.Cached;
using Infrastructure.Services;
using Infrastructure.TokenProviders;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using QuestPDF.Infrastructure;

namespace Infrastructure;

public static class DependencyInjection
{
    private const string _sectionName = "Redis";
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepositories();
        services.AddServices();

        services.AddScoped<UpdateAuditableEntitiesInterceptor>();
        services.AddScoped<ConvertDomainEventToOutboxMessagesInterceptor>();
        services.AddScoped<AuditLogInterceptor>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IAuthUnitOfWork>(sp => sp.GetRequiredService<AuthDbContext>());

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                .EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null))
                .AddInterceptors(
                  sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>(),
                  sp.GetRequiredService<ConvertDomainEventToOutboxMessagesInterceptor>(),
                  sp.GetRequiredService<AuditLogInterceptor>());
        });
        services.AddDbContext<AuthDbContext>((sp, options) =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("AuthConnection"),
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
        });

        QuestPDF.Settings.License = LicenseType.Community;

        services.Configure<InviteTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromHours(72);
        });

        services.AddIdentityCore<ApplicationUser>()
          .AddRoles<IdentityRole>()
          .AddEntityFrameworkStores<AuthDbContext>()
          .AddDefaultTokenProviders()
          .AddTokenProvider<InviteTokenProvider>(Application.Common.Constants.InviteTokenConstants.ProviderName);

        services.AddStackExchangeRedisCache(options =>
        {
            var redisOptions = configuration.GetSection(_sectionName).Get<RedisOptions>();
            options.Configuration = redisOptions!.ConnectionString;
        });

        services.Decorate<ICategoryRepository, CachedCategoryRepository>();
        services.Decorate<ISupplierRepository, CachedSupplierRepository>();

        services.ConfigureBackgroundJobs();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<INotificationSender<EmailMessageDto>, EmailSender>();
        services.AddScoped<IOrderPdfGenerator, OrderPdfGenerator>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();
        services.AddScoped<INotificationService, NotificationService>();
        return services;
    }

    public static IServiceCollection ConfigureBackgroundJobs(this IServiceCollection services)
    {
        services.AddQuartz(configure =>
        {
            var jobKey = JobKey.Create(nameof(ProcessOutboxMessagesJob));
            configure.AddJob<ProcessOutboxMessagesJob>(jobKey)
                        .AddTrigger(trigger => trigger.ForJob(jobKey)
                            .WithSimpleSchedule(x => x.WithIntervalInSeconds(10).RepeatForever()));
        });

        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        services.AddHostedService<ProcessOutboxEmailMessagesBackgroundService>();
        return services;
    }
}
