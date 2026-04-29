using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonsAndProgressSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lessons",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsPreviewable = table.Column<bool>(type: "bit", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonType = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResourcePublicId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: true),
                    VideoPublicId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ThumbnailPublicId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DurationInSeconds = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.CheckConstraint("CK_Lessons_DisplayOrder_Positive", "[DisplayOrder] > 0");
                    table.CheckConstraint("CK_ResourceLesson_FileSize_NonNegative", "[FileSizeInBytes] >= 0");
                    table.CheckConstraint("CK_VideoLesson_Duration_Positive", "[DurationInSeconds] > 0");
                    table.ForeignKey(
                        name: "FK_Lessons_CourseModules_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "courses",
                        principalTable: "CourseModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonAttachments",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonAttachments", x => x.Id);
                    table.CheckConstraint("CK_LessonAttachments_FileSize_NonNegative", "[FileSizeInBytes] >= 0");
                    table.ForeignKey(
                        name: "FK_LessonAttachments_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalSchema: "courses",
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonProgress",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonProgress", x => x.Id);
                    table.CheckConstraint("CK_LessonProgress_CompletedAt_WhenCompleted", "[IsCompleted] = 0 OR [CompletedAt] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_LessonProgress_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalSchema: "courses",
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonProgress_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonAttachments_IsDeleted",
                schema: "courses",
                table: "LessonAttachments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LessonAttachments_LessonId",
                schema: "courses",
                table: "LessonAttachments",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonAttachments_LessonId_PublicId",
                schema: "courses",
                table: "LessonAttachments",
                columns: new[] { "LessonId", "PublicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_CourseId",
                schema: "courses",
                table: "LessonProgress",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_IsDeleted",
                schema: "courses",
                table: "LessonProgress",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_LessonId",
                schema: "courses",
                table: "LessonProgress",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_ModuleId",
                schema: "courses",
                table: "LessonProgress",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_UserId_LessonId",
                schema: "courses",
                table: "LessonProgress",
                columns: new[] { "UserId", "LessonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_IsDeleted",
                schema: "courses",
                table: "Lessons",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_ModuleId",
                schema: "courses",
                table: "Lessons",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_ModuleId_DisplayOrder",
                schema: "courses",
                table: "Lessons",
                columns: new[] { "ModuleId", "DisplayOrder" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonAttachments",
                schema: "courses");

            migrationBuilder.DropTable(
                name: "LessonProgress",
                schema: "courses");

            migrationBuilder.DropTable(
                name: "Lessons",
                schema: "courses");
        }
    }
}
