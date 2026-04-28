using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseModulesReviewsAndEnrollments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseEnrollments",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrolledAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseEnrollments", x => x.Id);
                    table.CheckConstraint("CK_CourseEnrollments_PaidAmount_NonNegative", "[PaidAmount] >= 0");
                    table.CheckConstraint("CK_CourseEnrollments_Payment_Consistency", "([PaidAmount] = 0 AND [PaymentId] IS NULL) OR ([PaidAmount] > 0 AND [PaymentId] IS NOT NULL)");
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "courses",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseEnrollments_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseModules",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseModules", x => x.Id);
                    table.CheckConstraint("CK_CourseModules_DisplayOrder_Positive", "[DisplayOrder] > 0");
                    table.ForeignKey(
                        name: "FK_CourseModules_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "courses",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseReviews",
                schema: "courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(3,1)", nullable: false),
                    ReviewContent = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EditedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseReviews", x => x.Id);
                    table.CheckConstraint("CK_CourseReviews_Rating_Range", "[Rating] >= 1 AND [Rating] <= 5");
                    table.ForeignKey(
                        name: "FK_CourseReviews_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "courses",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseReviews_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_Categories_DisplayOrder_Positive",
                schema: "categories",
                table: "Categories",
                sql: "[DisplayOrder] > 0");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_CourseId",
                schema: "courses",
                table: "CourseEnrollments",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_IsDeleted",
                schema: "courses",
                table: "CourseEnrollments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_UserId_CourseId",
                schema: "courses",
                table: "CourseEnrollments",
                columns: new[] { "UserId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseModules_CourseId",
                schema: "courses",
                table: "CourseModules",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseModules_CourseId_DisplayOrder",
                schema: "courses",
                table: "CourseModules",
                columns: new[] { "CourseId", "DisplayOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseModules_IsDeleted",
                schema: "courses",
                table: "CourseModules",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CourseReviews_CourseId",
                schema: "courses",
                table: "CourseReviews",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseReviews_IsDeleted",
                schema: "courses",
                table: "CourseReviews",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_CourseReviews_UserId_CourseId",
                schema: "courses",
                table: "CourseReviews",
                columns: new[] { "UserId", "CourseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseEnrollments",
                schema: "courses");

            migrationBuilder.DropTable(
                name: "CourseModules",
                schema: "courses");

            migrationBuilder.DropTable(
                name: "CourseReviews",
                schema: "courses");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Categories_DisplayOrder_Positive",
                schema: "categories",
                table: "Categories");
        }
    }
}
