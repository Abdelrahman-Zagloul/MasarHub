using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MasarHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrdersAndPaymentsWithEnrollmentRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                schema: "courses",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_Payments_PaymentId",
                schema: "courses",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_UserId",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Provider",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Status",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_CourseEnrollments_PaymentId",
                schema: "courses",
                table: "CourseEnrollments");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CourseEnrollments_Payment_Consistency",
                schema: "courses",
                table: "CourseEnrollments");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                schema: "courses",
                table: "CourseEnrollments");

            migrationBuilder.EnsureSchema(
                name: "orders");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "payments",
                table: "Payments",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_UserId",
                schema: "payments",
                table: "Payments",
                newName: "IX_Payments_OrderId");

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                schema: "payments",
                table: "Payments",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "PaidAt",
                schema: "payments",
                table: "Payments",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CompletedAt",
                schema: "courses",
                table: "CourseEnrollments",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                schema: "courses",
                table: "CourseEnrollments",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "courses",
                table: "CourseEnrollments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Active");

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FinalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.CheckConstraint("CK_Orders_FinalAmount_NonNegative", "[FinalAmount] >= 0");
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                schema: "orders",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseTitle = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    OriginalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    FinalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CouponId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => new { x.OrderId, x.Id });
                    table.CheckConstraint("CK_OrderItems_Discount_Valid", "[DiscountAmount] <= [OriginalPrice]");
                    table.CheckConstraint("CK_OrderItems_FinalPrice_NonNegative", "[FinalPrice] >= 0");
                    table.CheckConstraint("CK_OrderItems_OriginalPrice_NonNegative", "[OriginalPrice] >= 0");
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "orders",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_IdempotencyKey",
                schema: "payments",
                table: "Payments",
                column: "IdempotencyKey",
                unique: true,
                filter: "[IdempotencyKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_OrderId",
                schema: "courses",
                table: "CourseEnrollments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId_CourseId",
                schema: "orders",
                table: "OrderItems",
                columns: new[] { "OrderId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_IsDeleted",
                schema: "orders",
                table: "Orders",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                schema: "orders",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status",
                schema: "orders",
                table: "Orders",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                schema: "orders",
                table: "Orders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                schema: "courses",
                table: "CourseEnrollments",
                column: "CourseId",
                principalSchema: "courses",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Orders_OrderId",
                schema: "courses",
                table: "CourseEnrollments",
                column: "OrderId",
                principalSchema: "orders",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId",
                schema: "payments",
                table: "Payments",
                column: "OrderId",
                principalSchema: "orders",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                schema: "courses",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseEnrollments_Orders_OrderId",
                schema: "courses",
                table: "CourseEnrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "OrderItems",
                schema: "orders");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "orders");

            migrationBuilder.DropIndex(
                name: "IX_Payments_IdempotencyKey",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_CourseEnrollments_OrderId",
                schema: "courses",
                table: "CourseEnrollments");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                schema: "courses",
                table: "CourseEnrollments");

            migrationBuilder.DropColumn(
                name: "OrderId",
                schema: "courses",
                table: "CourseEnrollments");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "courses",
                table: "CourseEnrollments");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                schema: "payments",
                table: "Payments",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_OrderId",
                schema: "payments",
                table: "Payments",
                newName: "IX_Payments_UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentId",
                schema: "courses",
                table: "CourseEnrollments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Provider",
                schema: "payments",
                table: "Payments",
                column: "Provider");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Status",
                schema: "payments",
                table: "Payments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CourseEnrollments_PaymentId",
                schema: "courses",
                table: "CourseEnrollments",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CourseEnrollments_Payment_Consistency",
                schema: "courses",
                table: "CourseEnrollments",
                sql: "([PaidAmount] = 0 AND [PaymentId] IS NULL) OR ([PaidAmount] > 0 AND [PaymentId] IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Courses_CourseId",
                schema: "courses",
                table: "CourseEnrollments",
                column: "CourseId",
                principalSchema: "courses",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEnrollments_Payments_PaymentId",
                schema: "courses",
                table: "CourseEnrollments",
                column: "PaymentId",
                principalSchema: "payments",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_UserId",
                schema: "payments",
                table: "Payments",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
