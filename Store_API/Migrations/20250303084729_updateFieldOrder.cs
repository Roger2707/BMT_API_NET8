using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store_API.Migrations
{
    /// <inheritdoc />
    public partial class updateFieldOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalItemsPrice",
                table: "OrderItems");

            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "OrderItems",
                newName: "SubTotal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubTotal",
                table: "OrderItems",
                newName: "UnitPrice");

            migrationBuilder.AddColumn<double>(
                name: "TotalItemsPrice",
                table: "OrderItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
