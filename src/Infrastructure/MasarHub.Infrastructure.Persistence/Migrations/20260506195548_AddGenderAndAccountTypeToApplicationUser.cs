using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderAndAccountTypeToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountStatus",
                schema: "identity",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                schema: "identity",
                table: "Users",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountStatus",
                schema: "identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "identity",
                table: "Users");
        }
    }
}
