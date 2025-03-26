using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store_API.Migrations
{
    /// <inheritdoc />
    public partial class updateColumnsPromotion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Promotions",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "Promotions",
                newName: "EndDate");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d9285409-6bda-454d-acfd-4e1407cd6e9c", "AQAAAAIAAYagAAAAEK+GahMv1y7O58nyL7Cg3bqMmOH1ejsAMFEFQevmxEdQV57kddg1x1J3zejtOp0vmA==", "c3f91281-b9d4-4fce-a2bd-2e61575c4e7d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Promotions",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Promotions",
                newName: "End");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ff141ff4-d112-4996-a899-426e1b9375f4", "AQAAAAIAAYagAAAAEDbfMi8AnrILiP7A6xEX8B6k+/ZGQcMi4XB+TRUOwKYYWuB/HkGw7he+//up+rwTug==", "5ca4edd7-47a6-44a3-a04e-ae17dba020b7" });
        }
    }
}
