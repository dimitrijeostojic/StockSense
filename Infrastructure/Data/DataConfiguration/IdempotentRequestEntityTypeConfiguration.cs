using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DataConfiguration;

internal sealed class IdempotentRequestEntityTypeConfiguration : IEntityTypeConfiguration<IdempotentRequest>
{
    public void Configure(EntityTypeBuilder<IdempotentRequest> builder)
    {
        builder.ToTable("idempotent_requests");
        builder.HasKey(ir => ir.RequestId);
        builder.HasIndex(ir => ir.RequestId).IsUnique();
        builder.Property(ir => ir.Name).IsRequired();
    }
}
