using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixCategorySlugUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentCategoryId_Slug",
                schema: "categories",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                schema: "categories",
                table: "Categories",
                column: "Slug",
                unique: true,
                filter: "[ParentCategoryId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId_Slug",
                schema: "categories",
                table: "Categories",
                columns: new[] { "ParentCategoryId", "Slug" },
                unique: true,
                filter: "[ParentCategoryId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Categories_Slug",
                schema: "categories",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentCategoryId_Slug",
                schema: "categories",
                table: "Categories");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId_Slug",
                schema: "categories",
                table: "Categories",
                columns: new[] { "ParentCategoryId", "Slug" },
                unique: true);
        }
    }
}
