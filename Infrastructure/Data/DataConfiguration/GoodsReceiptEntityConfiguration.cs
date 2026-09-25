using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DataConfiguration;

internal sealed class GoodsReceiptEntityConfiguration : IEntityTypeConfiguration<GoodsReceipt>
{
    public void Configure(EntityTypeBuilder<GoodsReceipt> builder)
    {
        builder.ToTable("GoodsReceipts");

        builder.HasKey(gr => gr.Id);
        builder.Property(gr => gr.OrderId).IsRequired();
        builder.Property(gr => gr.OrderPublicId).IsRequired();
        builder.Property(gr => gr.TenantPublicId).IsRequired();
        builder.Property(gr => gr.ReceivedAt).IsRequired();
        builder.Property(gr => gr.Notes).HasMaxLength(500);

        builder.HasOne<Order>()
            .WithOne()
            .HasForeignKey<GoodsReceipt>(gr => gr.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(gr => gr.Items)
            .WithOne()
            .HasForeignKey(i => i.GoodsReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(gr => gr.OrderPublicId).IsUnique();
        builder.HasIndex(gr => gr.TenantPublicId);
    }
}
