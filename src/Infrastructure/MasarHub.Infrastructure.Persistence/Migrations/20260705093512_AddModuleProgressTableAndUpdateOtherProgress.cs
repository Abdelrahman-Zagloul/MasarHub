using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddModuleProgressTableAndUpdateOtherProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_LessonProgress_CompletedAt_WhenCompleted",
                schema: "courses",
                table: "LessonProgress");

            migrationBuilder.DropIndex(
                name: "IX_CourseProgress_IsCompleted",
                schema: "courses",
                table: "CourseProgress");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CourseProgress_Percentage_Range",
                schema: "courses",
                table: "CourseProgress");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                schema: "courses",
                table: "LessonProgress");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                schema: "courses",
                table: "LessonProgress");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                schema: "courses",
                table: "CourseProgress");

            migrationBuilder.DropColumn(
                name: "ProgressPercentage",
                schema: "courses",
                table: "CourseProgress");

            migrationBuilder.CreateTable(
                name: "ModuleProgress",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompletedLessons = table.Column<int>(type: "int", nullable: false),
                    TotalLessons = table.Column<int>(type: "int", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleProgress", x => x.Id);
                    table.CheckConstraint("CK_ModuleProgress_CompletedLessons_NonNegative", "[CompletedLessons] >= 0");
                    table.CheckConstraint("CK_ModuleProgress_TotalLessons_Positive", "[TotalLessons] > 0");
                    table.CheckConstraint("CK_ModuleProgress_TotalLessonsLessThanCompleted", "[TotalLessons] >= [CompletedLessons]");
                    table.ForeignKey(
                        name: "FK_ModuleProgress_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "courses",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_CourseId",
                schema: "courses",
                table: "ModuleProgress",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_IsDeleted",
                schema: "courses",
                table: "ModuleProgress",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_ModuleId",
                schema: "courses",
                table: "ModuleProgress",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_UserId_CourseId",
                schema: "courses",
                table: "ModuleProgress",
                columns: new[] { "UserId", "CourseId" });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleProgress_UserId_ModuleId",
                schema: "courses",
                table: "ModuleProgress",
                columns: new[] { "UserId", "ModuleId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleProgress",
                schema: "courses");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedAt",
                schema: "courses",
                table: "LessonProgress",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                schema: "courses",
                table: "LessonProgress",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                schema: "courses",
                table: "CourseProgress",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ProgressPercentage",
                schema: "courses",
                table: "CourseProgress",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddCheckConstraint(
                name: "CK_LessonProgress_CompletedAt_WhenCompleted",
                schema: "courses",
                table: "LessonProgress",
                sql: "[IsCompleted] = 0 OR [CompletedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CourseProgress_IsCompleted",
                schema: "courses",
                table: "CourseProgress",
                column: "IsCompleted");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CourseProgress_Percentage_Range",
                schema: "courses",
                table: "CourseProgress",
                sql: "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
        }
    }
}
