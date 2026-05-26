using Domain.Entities;
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
        builder.Property(c => c.CreatedBy).IsRequired().HasMaxLength(255);
        builder.Property(c => c.ModifiedBy).HasMaxLength(255);

        builder.HasOne(o => o.Supplier)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}
