using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorPaymentIdentifiers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_IdempotencyKey",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Provider_ExternalId",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payments_ExternalId_NotNull_WhenSucceeded",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                schema: "payments",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "IdempotencyKey",
                schema: "payments",
                table: "Payments",
                newName: "ProviderReference");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Provider_ProviderReference",
                schema: "payments",
                table: "Payments",
                columns: new[] { "Provider", "ProviderReference" },
                unique: true,
                filter: "[ProviderReference] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payments_ProviderReference_NotNull_WhenSucceeded",
                schema: "payments",
                table: "Payments",
                sql: "[Status] <> 'Succeeded' OR [ProviderReference] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_Provider_ProviderReference",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Payments_ProviderReference_NotNull_WhenSucceeded",
                schema: "payments",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "ProviderReference",
                schema: "payments",
                table: "Payments",
                newName: "IdempotencyKey");

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                schema: "payments",
                table: "Payments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_IdempotencyKey",
                schema: "payments",
                table: "Payments",
                column: "IdempotencyKey",
                unique: true,
                filter: "[IdempotencyKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Provider_ExternalId",
                schema: "payments",
                table: "Payments",
                columns: new[] { "Provider", "ExternalId" },
                unique: true,
                filter: "[ExternalId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Payments_ExternalId_NotNull_WhenSucceeded",
                schema: "payments",
                table: "Payments",
                sql: "[Status] <> 'Succeeded' OR [ExternalId] IS NOT NULL");
        }
    }
}
