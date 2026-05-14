using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPreferredTwoFactorProviderToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreferredTwoFactorProvider",
                schema: "identity",
                table: "Users",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreferredTwoFactorProvider",
                schema: "identity",
                table: "Users");
        }
    }
}
