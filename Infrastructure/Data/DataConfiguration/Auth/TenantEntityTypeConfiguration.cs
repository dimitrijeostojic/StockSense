using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DataConfiguration.Auth;

internal sealed class TenantEntityTypeConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Name).IsRequired().HasMaxLength(255);
        builder.Property(o => o.Pib).IsRequired().HasMaxLength(255);
        builder.Property(o => o.Address).HasMaxLength(255);
        builder.Property(o => o.Logo).HasColumnType("varbinary(max)");

        builder.HasMany(t => t.ApplicationUsers)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.PublicId);
        builder.HasIndex(t => t.Pib).IsUnique();
    }
}
