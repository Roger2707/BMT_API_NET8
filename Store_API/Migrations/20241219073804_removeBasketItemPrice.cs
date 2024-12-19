using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store_API.Migrations
{
    /// <inheritdoc />
    public partial class removeBasketItemPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "BasketItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "BasketItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
