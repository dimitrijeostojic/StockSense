using Domain.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Infrastructure.Data;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<ApplicationUser>(options), IAuthUnitOfWork
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        #region Tenants
        builder.Entity<Tenant>().ToTable("Tenants");
        builder.Entity<Tenant>().HasKey(o => o.Id);
        builder.Entity<Tenant>().Property(o => o.Name).IsRequired().HasMaxLength(255);
        builder.Entity<Tenant>().Property(o => o.PIB).IsRequired().HasMaxLength(255);
        builder.Entity<Tenant>().Property(o => o.Address).IsRequired().HasMaxLength(255);
        builder.Entity<Tenant>()
            .HasMany(t => t.ApplicationUsers)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion

        #region RefreshToken
        builder.Entity<RefreshToken>().ToTable("RefreshTokens");
        builder.Entity<RefreshToken>().HasKey(rt => rt.Id);
        builder.Entity<RefreshToken>().Property(rt => rt.Token).HasMaxLength(255);
        builder.Entity<RefreshToken>().HasIndex(rt => rt.Token);
        builder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion

        var adminRoleId = "2d122bc9-28fa-45eb-b4ea-9d494904cd7f";
        var userRoleId = "0da6a4ef-163a-4fe2-80a6-0925b765efce";
        var roles = new List<IdentityRole>
        {
            new() { Id = adminRoleId, ConcurrencyStamp=adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
            new() { Id = userRoleId, ConcurrencyStamp=userRoleId, Name = "User", NormalizedName = "USER" }
        };
        builder.Entity<IdentityRole>().HasData(roles);
    }

    public IDbTransaction BeginTransaction()
    {
        var transaction = Database.BeginTransaction();
        return transaction.GetDbTransaction();
    }
}
