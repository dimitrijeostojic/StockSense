using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DataConfiguration;

internal sealed class SupplierEntityConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(100).IsRequired();
        builder.Property(s => s.ContactName).HasMaxLength(100).IsRequired();
        builder.Property(s => s.ContactEmail).HasMaxLength(100);
        builder.Property(s => s.ContactPhone).HasMaxLength(100).IsRequired();
        builder.Property(c => c.CreatedBy).IsRequired().HasMaxLength(255);
        builder.Property(c => c.ModifiedBy).HasMaxLength(255);

    }
}
