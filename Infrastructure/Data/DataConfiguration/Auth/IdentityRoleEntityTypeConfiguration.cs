using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.DataConfiguration.Auth;

internal sealed class IdentityRoleEntityTypeConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        var adminRoleId = "2d122bc9-28fa-45eb-b4ea-9d494904cd7f";
        var userRoleId = "0da6a4ef-163a-4fe2-80a6-0925b765efce";
        var roles = new List<IdentityRole>
        {
            new() { Id = adminRoleId, ConcurrencyStamp=adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
            new() { Id = userRoleId, ConcurrencyStamp=userRoleId, Name = "User", NormalizedName = "USER" }
        };
        builder.HasData(roles);
    }
}
