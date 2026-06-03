using Domain.Abstractions;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Infrastructure.Data;

public sealed class AuthDbContext(DbContextOptions<AuthDbContext> options) : IdentityDbContext<ApplicationUser>(options), IAuthUnitOfWork
{
    public DbSet<Tenant> Tenants { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
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
    }

    public IDbTransaction BeginTransaction()
    {
        var transaction = Database.BeginTransaction();
        return transaction.GetDbTransaction();
    }
}
