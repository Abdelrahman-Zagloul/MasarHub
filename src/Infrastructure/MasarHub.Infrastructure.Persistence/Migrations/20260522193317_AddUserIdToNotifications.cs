using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TargetRole",
                schema: "notifications",
                table: "Notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "notifications",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                schema: "notifications",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Notifications_TargetRole_Or_UserId",
                schema: "notifications",
                table: "Notifications",
                sql: "[TargetRole] IS NOT NULL OR [UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Notifications_UserId",
                schema: "notifications",
                table: "Notifications");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Notifications_TargetRole_Or_UserId",
                schema: "notifications",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "notifications",
                table: "Notifications");

            migrationBuilder.AlterColumn<string>(
                name: "TargetRole",
                schema: "notifications",
                table: "Notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
