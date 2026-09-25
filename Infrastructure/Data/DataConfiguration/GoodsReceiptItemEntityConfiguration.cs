using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DataConfiguration;

internal sealed class GoodsReceiptItemEntityConfiguration : IEntityTypeConfiguration<GoodsReceiptItem>
{
    public void Configure(EntityTypeBuilder<GoodsReceiptItem> builder)
    {
        builder.ToTable("GoodsReceiptItems");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.GoodsReceiptId).IsRequired();
        builder.Property(i => i.OrderItemId).IsRequired();
        builder.Property(i => i.ProductId).IsRequired();
        builder.Property(i => i.OrderedQuantity).IsRequired();
        builder.Property(i => i.ReceivedQuantity).IsRequired();

        builder.HasOne(i => i.OrderItem)
            .WithOne()
            .HasForeignKey<GoodsReceiptItem>(i => i.OrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.GoodsReceiptId);
        builder.HasIndex(i => i.OrderItemId).IsUnique();
    }
}
