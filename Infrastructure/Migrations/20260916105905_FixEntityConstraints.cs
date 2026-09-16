using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixEntityConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_PublicId",
                table: "Suppliers",
                column: "PublicId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_TenantPublicId_SupplierCode",
                table: "Suppliers",
                columns: new[] { "TenantPublicId", "SupplierCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantPublicId",
                table: "Products",
                column: "TenantPublicId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_PublicId",
                table: "Orders",
                column: "PublicId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId_ProductId",
                table: "OrderItems",
                columns: new[] { "OrderId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantPublicId_Name",
                table: "Categories",
                columns: new[] { "TenantPublicId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Suppliers_PublicId",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_TenantPublicId_SupplierCode",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantPublicId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Orders_PublicId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_OrderId_ProductId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Categories_TenantPublicId_Name",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }
    }
}
