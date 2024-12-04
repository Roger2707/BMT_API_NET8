using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store_API.Migrations
{
    /// <inheritdoc />
    public partial class updateTableAddressName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_AspNetUsers_UserId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Addresses_UserAddressId",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "Addresses",
                newName: "UserAddresses");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_UserId",
                table: "UserAddresses",
                newName: "IX_UserAddresses_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAddresses",
                table: "UserAddresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_UserAddresses_UserAddressId",
                table: "Orders",
                column: "UserAddressId",
                principalTable: "UserAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddresses_AspNetUsers_UserId",
                table: "UserAddresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_UserAddresses_UserAddressId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAddresses_AspNetUsers_UserId",
                table: "UserAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAddresses",
                table: "UserAddresses");

            migrationBuilder.RenameTable(
                name: "UserAddresses",
                newName: "Addresses");

            migrationBuilder.RenameIndex(
                name: "IX_UserAddresses_UserId",
                table: "Addresses",
                newName: "IX_Addresses_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_AspNetUsers_UserId",
                table: "Addresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Addresses_UserAddressId",
                table: "Orders",
                column: "UserAddressId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
