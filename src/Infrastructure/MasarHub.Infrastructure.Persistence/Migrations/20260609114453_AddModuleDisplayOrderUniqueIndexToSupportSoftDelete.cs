using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddModuleDisplayOrderUniqueIndexToSupportSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CourseModules_CourseId_DisplayOrder",
                schema: "courses",
                table: "CourseModules");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CourseModules_DisplayOrder_Positive",
                schema: "courses",
                table: "CourseModules");

            migrationBuilder.CreateIndex(
                name: "IX_CourseModules_CourseId_DisplayOrder",
                schema: "courses",
                table: "CourseModules",
                columns: new[] { "CourseId", "DisplayOrder" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CourseModules_DisplayOrder_Positive",
                schema: "courses",
                table: "CourseModules",
                sql: "([IsDeleted] = 0 AND [DisplayOrder] > 0) OR ([IsDeleted] = 1 AND [DisplayOrder] = 0)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CourseModules_CourseId_DisplayOrder",
                schema: "courses",
                table: "CourseModules");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CourseModules_DisplayOrder_Positive",
                schema: "courses",
                table: "CourseModules");

            migrationBuilder.CreateIndex(
                name: "IX_CourseModules_CourseId_DisplayOrder",
                schema: "courses",
                table: "CourseModules",
                columns: new[] { "CourseId", "DisplayOrder" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_CourseModules_DisplayOrder_Positive",
                schema: "courses",
                table: "CourseModules",
                sql: "[DisplayOrder] > 0");
        }
    }
}
