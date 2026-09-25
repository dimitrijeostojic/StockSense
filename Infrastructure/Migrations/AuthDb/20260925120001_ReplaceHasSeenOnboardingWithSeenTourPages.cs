using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class ReplaceHasSeenOnboardingWithSeenTourPages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Migrate existing data: users who completed onboarding get "dashboard" in their tour pages
            migrationBuilder.Sql(
                "UPDATE [AspNetUsers] SET [SeenTourPages] = '[\"dashboard\"]' WHERE [HasSeenOnboarding] = 1 AND [SeenTourPages] IS NULL");

            migrationBuilder.DropColumn(
                name: "HasSeenOnboarding",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasSeenOnboarding",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(
                "UPDATE [AspNetUsers] SET [HasSeenOnboarding] = 1 WHERE [SeenTourPages] LIKE '%\"dashboard\"%'");
        }
    }
}
