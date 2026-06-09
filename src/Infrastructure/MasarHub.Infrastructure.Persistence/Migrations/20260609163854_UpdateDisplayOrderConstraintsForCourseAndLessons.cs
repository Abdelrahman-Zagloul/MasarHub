using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDisplayOrderConstraintsForCourseAndLessons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lessons_ModuleId_DisplayOrder",
                schema: "courses",
                table: "Lessons");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Lessons_DisplayOrder_Positive",
                schema: "courses",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_CourseModules_CourseId_DisplayOrder",
                schema: "courses",
                table: "CourseModules");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CourseModules_DisplayOrder_Positive",
                schema: "courses",
                table: "CourseModules");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPreviewable",
                schema: "courses",
                table: "Lessons",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_ModuleId_DisplayOrder",
                schema: "courses",
                table: "Lessons",
                columns: new[] { "ModuleId", "DisplayOrder" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Lessons_DisplayOrder_Positive",
                schema: "courses",
                table: "Lessons",
                sql: "([IsDeleted] = 0 AND [DisplayOrder] > 0) OR ([IsDeleted] = 1 AND [DisplayOrder] = 0)");

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
                name: "IX_Lessons_ModuleId_DisplayOrder",
                schema: "courses",
                table: "Lessons");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Lessons_DisplayOrder_Positive",
                schema: "courses",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_CourseModules_CourseId_DisplayOrder",
                schema: "courses",
                table: "CourseModules");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CourseModules_DisplayOrder_Positive",
                schema: "courses",
                table: "CourseModules");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPreviewable",
                schema: "courses",
                table: "Lessons",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_ModuleId_DisplayOrder",
                schema: "courses",
                table: "Lessons",
                columns: new[] { "ModuleId", "DisplayOrder" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Lessons_DisplayOrder_Positive",
                schema: "courses",
                table: "Lessons",
                sql: "[DisplayOrder] > 0");

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
