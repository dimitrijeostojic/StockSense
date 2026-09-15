using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_TenantPublicId",
                table: "Suppliers",
                column: "TenantPublicId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_PublicId",
                table: "Products",
                column: "PublicId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantPublicId_Sku",
                table: "Products",
                columns: new[] { "TenantPublicId", "Sku" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TenantPublicId",
                table: "Orders",
                column: "TenantPublicId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_PublicId",
                table: "Categories",
                column: "PublicId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantPublicId",
                table: "Categories",
                column: "TenantPublicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Suppliers_TenantPublicId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Products_PublicId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantPublicId_Sku",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TenantPublicId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Categories_PublicId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_TenantPublicId",
                table: "Categories");
        }
    }
}
