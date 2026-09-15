using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class ExtractConfigurationFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tenants_Pib",
                table: "Tenants",
                column: "Pib",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_PublicId",
                table: "Tenants",
                column: "PublicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tenants_Pib",
                table: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Tenants_PublicId",
                table: "Tenants");
        }
    }
}
