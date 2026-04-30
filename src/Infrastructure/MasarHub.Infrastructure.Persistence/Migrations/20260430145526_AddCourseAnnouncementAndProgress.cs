using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseAnnouncementAndProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseAnnouncements",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstructorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PublishedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ScheduledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Importance = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsPinned = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseAnnouncements", x => x.Id);
                    table.CheckConstraint("CK_Announcement_PublishedAt_WhenPublished", "[IsPublished] = 0 OR [PublishedAt] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_CourseAnnouncements_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "courses",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseProgress",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompletedLessons = table.Column<int>(type: "int", nullable: false),
                    TotalLessons = table.Column<int>(type: "int", nullable: false),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseProgress", x => x.Id);
                    table.CheckConstraint("CK_CourseProgress_CompletedLessons_NonNegative", "[CompletedLessons] >= 0");
                    table.CheckConstraint("CK_CourseProgress_Percentage_Range", "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");
                    table.CheckConstraint("CK_CourseProgress_TotalLessons_Positive", "[TotalLessons] > 0");
                    table.CheckConstraint("CK_CourseProgress_TotalLessonsLessThanCompleted", "[TotalLessons] >= [CompletedLessons]");
                    table.ForeignKey(
                        name: "FK_CourseProgress_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "courses",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseAnnouncements_CourseId",
                schema: "courses",
                table: "CourseAnnouncements",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAnnouncements_CourseId_IsPinned",
                schema: "courses",
                table: "CourseAnnouncements",
                columns: new[] { "CourseId", "IsPinned" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseAnnouncements_CourseId_IsPinned_Importance_PublishedAt",
                schema: "courses",
                table: "CourseAnnouncements",
                columns: new[] { "CourseId", "IsPinned", "Importance", "PublishedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseAnnouncements_CourseId_IsPublished",
                schema: "courses",
                table: "CourseAnnouncements",
                columns: new[] { "CourseId", "IsPublished" });

            migrationBuilder.CreateIndex(
                name: "IX_CourseAnnouncements_IsDeleted",
                schema: "courses",
                table: "CourseAnnouncements",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CourseProgress_CourseId",
                schema: "courses",
                table: "CourseProgress",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseProgress_IsCompleted",
                schema: "courses",
                table: "CourseProgress",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_CourseProgress_IsDeleted",
                schema: "courses",
                table: "CourseProgress",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CourseProgress_UserId",
                schema: "courses",
                table: "CourseProgress",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseProgress_UserId_CourseId",
                schema: "courses",
                table: "CourseProgress",
                columns: new[] { "UserId", "CourseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseAnnouncements",
                schema: "courses");

            migrationBuilder.DropTable(
                name: "CourseProgress",
                schema: "courses");
        }
    }
}
