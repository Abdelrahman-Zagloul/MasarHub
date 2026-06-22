using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCouponUniqueIndexToCodeAndCourseId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Coupons_Code",
                schema: "payments",
                table: "Coupons");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code_CourseId",
                schema: "payments",
                table: "Coupons",
                columns: new[] { "Code", "CourseId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Coupons_Code_CourseId",
                schema: "payments",
                table: "Coupons");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                schema: "payments",
                table: "Coupons",
                column: "Code",
                unique: true);
        }
    }
}
