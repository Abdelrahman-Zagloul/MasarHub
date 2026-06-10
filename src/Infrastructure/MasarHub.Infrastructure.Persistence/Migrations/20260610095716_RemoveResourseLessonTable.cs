using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveResourseLessonTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_VideoLesson_Duration_Positive",
                schema: "courses",
                table: "Lessons");

            migrationBuilder.DropCheckConstraint(
                name: "CK_ResourceLesson_FileSize_NonNegative",
                schema: "courses",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "FileType",
                schema: "courses",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "ResourcePublicId",
                schema: "courses",
                table: "Lessons");

            migrationBuilder.RenameColumn(
                name: "FileSizeInBytes",
                schema: "courses",
                table: "Lessons",
                newName: "FileSizeInByte");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                schema: "courses",
                table: "Lessons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DurationInSeconds",
                schema: "courses",
                table: "Lessons",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_VideoLesson_FileSize_Positive",
                schema: "courses",
                table: "Lessons",
                sql: "[FileSizeInByte] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_VideoLesson_Duration_Positive",
                schema: "courses",
                table: "Lessons",
                sql: "[DurationInSeconds] > 0");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_VideoLesson_Duration_Positive",
                schema: "courses",
                table: "Lessons");

            migrationBuilder.DropCheckConstraint(
                name: "CK_VideoLesson_FileSize_Positive",
                schema: "courses",
                table: "Lessons");

            migrationBuilder.RenameColumn(
                name: "FileSizeInByte",
                schema: "courses",
                table: "Lessons",
                newName: "FileSizeInBytes");

            migrationBuilder.AlterColumn<string>(
                name: "FileName",
                schema: "courses",
                table: "Lessons",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DurationInSeconds",
                schema: "courses",
                table: "Lessons",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                schema: "courses",
                table: "Lessons",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourcePublicId",
                schema: "courses",
                table: "Lessons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_ResourceLesson_FileSize_NonNegative",
                schema: "courses",
                table: "Lessons",
                sql: "[FileSizeInBytes] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_VideoLesson_Duration_Positive",
                schema: "courses",
                table: "Lessons",
                sql: "[DurationInSeconds] > 0");
        }
    }
}
