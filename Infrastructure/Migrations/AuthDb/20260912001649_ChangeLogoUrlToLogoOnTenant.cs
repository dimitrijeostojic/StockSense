using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class ChangeLogoUrlToLogoOnTenant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Tenants");

            migrationBuilder.AddColumn<byte[]>(
                name: "Logo",
                table: "Tenants",
                type: "varbinary(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Tenants");

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Tenants",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
