using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExamModuleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "exams");

            migrationBuilder.CreateTable(
                name: "Exams",
                schema: "exams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    PassingScorePercentage = table.Column<int>(type: "int", nullable: false),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: true),
                    MaxAttempts = table.Column<int>(type: "int", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exams", x => x.Id);
                    table.CheckConstraint("CK_Exams_Duration_Positive", "[DurationInMinutes] IS NULL OR [DurationInMinutes] > 0");
                    table.CheckConstraint("CK_Exams_MaxAttempts_Positive", "[MaxAttempts] > 0");
                    table.CheckConstraint("CK_Exams_PassingScore_Range", "[PassingScorePercentage] >= 0 AND [PassingScorePercentage] <= 100");
                    table.ForeignKey(
                        name: "FK_Exams_CourseModules_ModuleId",
                        column: x => x.ModuleId,
                        principalSchema: "courses",
                        principalTable: "CourseModules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Exams_Courses_CourseId",
                        column: x => x.CourseId,
                        principalSchema: "courses",
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamAttempts",
                schema: "exams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SubmittedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Score = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamAttempts", x => x.Id);
                    table.CheckConstraint("CK_ExamAttempts_Score_NonNegative", "[Score] >= 0");
                    table.CheckConstraint("CK_ExamAttempts_SubmittedAt_When_Submitted", "[Status] <> 'Submitted' OR [SubmittedAt] IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_ExamAttempts_Exams_ExamId",
                        column: x => x.ExamId,
                        principalSchema: "exams",
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                schema: "exams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    QuestionMark = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuestionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.CheckConstraint("CK_Questions_Mark_Positive", "[QuestionMark] > 0");
                    table.ForeignKey(
                        name: "FK_Questions_Exams_ExamId",
                        column: x => x.ExamId,
                        principalSchema: "exams",
                        principalTable: "Exams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamAnswers",
                schema: "exams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamAttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnsweredAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamAnswers_ExamAttempts_ExamAttemptId",
                        column: x => x.ExamAttemptId,
                        principalSchema: "exams",
                        principalTable: "ExamAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "exams",
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Options",
                schema: "exams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Options", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Options_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "exams",
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamAnswerOptions",
                schema: "exams",
                columns: table => new
                {
                    ExamAnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamAnswerOptions", x => new { x.ExamAnswerId, x.OptionId });
                    table.ForeignKey(
                        name: "FK_ExamAnswerOptions_ExamAnswers_ExamAnswerId",
                        column: x => x.ExamAnswerId,
                        principalSchema: "exams",
                        principalTable: "ExamAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamAnswerOptions_Options_OptionId",
                        column: x => x.OptionId,
                        principalSchema: "exams",
                        principalTable: "Options",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswerOptions_ExamAnswerId",
                schema: "exams",
                table: "ExamAnswerOptions",
                column: "ExamAnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswerOptions_OptionId",
                schema: "exams",
                table: "ExamAnswerOptions",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswers_ExamAttemptId",
                schema: "exams",
                table: "ExamAnswers",
                column: "ExamAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswers_ExamAttemptId_QuestionId",
                schema: "exams",
                table: "ExamAnswers",
                columns: new[] { "ExamAttemptId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswers_IsDeleted",
                schema: "exams",
                table: "ExamAnswers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswers_QuestionId",
                schema: "exams",
                table: "ExamAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttempts_ExamId_UserId",
                schema: "exams",
                table: "ExamAttempts",
                columns: new[] { "ExamId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttempts_IsDeleted",
                schema: "exams",
                table: "ExamAttempts",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAttempts_Status",
                schema: "exams",
                table: "ExamAttempts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_CourseId",
                schema: "exams",
                table: "Exams",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_IsDeleted",
                schema: "exams",
                table: "Exams",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Exams_ModuleId",
                schema: "exams",
                table: "Exams",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_Options_QuestionId",
                schema: "exams",
                table: "Options",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ExamId",
                schema: "exams",
                table: "Questions",
                column: "ExamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamAnswerOptions",
                schema: "exams");

            migrationBuilder.DropTable(
                name: "ExamAnswers",
                schema: "exams");

            migrationBuilder.DropTable(
                name: "Options",
                schema: "exams");

            migrationBuilder.DropTable(
                name: "ExamAttempts",
                schema: "exams");

            migrationBuilder.DropTable(
                name: "Questions",
                schema: "exams");

            migrationBuilder.DropTable(
                name: "Exams",
                schema: "exams");
        }
    }
}
