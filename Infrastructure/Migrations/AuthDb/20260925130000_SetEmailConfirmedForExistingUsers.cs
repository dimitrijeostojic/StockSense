using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.AuthDb
{
    /// <inheritdoc />
    public partial class SetEmailConfirmedForExistingUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE AspNetUsers SET EmailConfirmed = 1 WHERE EmailConfirmed = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally not reversed: cannot know which users had EmailConfirmed=false before this ran.
        }
    }
}
