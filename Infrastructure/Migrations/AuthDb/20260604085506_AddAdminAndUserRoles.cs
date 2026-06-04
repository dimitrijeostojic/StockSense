using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class AddAdminAndUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0da6a4ef-163a-4fe2-80a6-0925b765efce", "0da6a4ef-163a-4fe2-80a6-0925b765efce", "User", "USER" },
                    { "2d122bc9-28fa-45eb-b4ea-9d494904cd7f", "2d122bc9-28fa-45eb-b4ea-9d494904cd7f", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0da6a4ef-163a-4fe2-80a6-0925b765efce");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2d122bc9-28fa-45eb-b4ea-9d494904cd7f");
        }
    }
}
