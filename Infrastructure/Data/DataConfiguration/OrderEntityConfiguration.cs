using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DataConfiguration;

internal sealed class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.SupplierId).IsRequired();
        builder.Property(o => o.OrderDate).IsRequired();
        builder.Property(o => o.Notes).HasMaxLength(255);
        builder.Property(o => o.OrderStatus).IsRequired();
        builder.Property(o => o.Currency).IsRequired().HasDefaultValue(Currency.EUR);
        builder.Property(o => o.CreatedBy).IsRequired().HasMaxLength(255);
        builder.Property(o => o.ModifiedBy).HasMaxLength(255);

        builder.HasOne(o => o.Supplier)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.TenantPublicId);
        builder.HasIndex(o => o.PublicId);
    }
}
