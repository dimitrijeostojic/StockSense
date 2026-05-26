using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DataConfiguration;

internal sealed class StockEntryEntityConfiguration : IEntityTypeConfiguration<StockEntry>
{
    public void Configure(EntityTypeBuilder<StockEntry> builder)
    {
        builder.ToTable("StockEntries");

        builder.HasKey(se => se.Id);
        builder.Property(se => se.Quantity).IsRequired();
        builder.Property(se => se.EntryDate).IsRequired();
        builder.Property(se => se.ProductId).IsRequired();
        builder.Property(se => se.StockEntryType).IsRequired();
        builder.Property(se => se.Notes).HasMaxLength(255);

        builder.HasOne(se => se.Product)
            .WithMany(p => p.StockEntries)
            .HasForeignKey(se => se.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
