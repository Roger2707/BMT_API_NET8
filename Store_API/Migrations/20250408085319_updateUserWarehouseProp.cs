using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store_API.Migrations
{
    /// <inheritdoc />
    public partial class updateUserWarehouseProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWarehouses_AspNetRoles_RoleId",
                table: "UserWarehouses");

            migrationBuilder.DropIndex(
                name: "IX_UserWarehouses_RoleId",
                table: "UserWarehouses");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "UserWarehouses");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6246a07c-9408-4551-a834-96b40c7600bb", "AQAAAAIAAYagAAAAECFcJ7fADsyVG/FQH9KidZvWBHv8z/7CIVUVQ5/qSOHFqFVfcHrhILfeioqHvEVa4w==", "3320b456-75b3-41f5-a7be-6131466ba1b5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "36f7b4c1-d72e-45e6-98ab-1f42bb7dba4b", "AQAAAAIAAYagAAAAEOEeCj2OGi6SEy2cKvxAzgkrWeYM4J0YIJ0dtBfDjY1zHtLjmAcLnREQeG364HUUxA==", "7deb2e65-22bc-499d-83f2-f437b283749b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "05e79f23-49de-46ec-80d8-27763f0670de", "AQAAAAIAAYagAAAAEA7535PHsoQouTYrFwcVhnFtea9BT7pr+aA+m3TB5BBZ8M0uky3FdR8EaUptYBHNeg==", "f58af746-fb30-4972-95b8-7e395d5241f2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aadf0dfe-7194-4f00-a055-d292380f30f3", "AQAAAAIAAYagAAAAEIvrHs0w+1rA/RDCrA/MJkeFXw/iIFkM/Geoag/EBDMaNVG5SCPGwGpQ9z16HCPvfA==", "6f44406c-1ea9-4976-b21f-cb5be972f077" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "UserWarehouses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a17d37cd-c106-482c-b4b0-cb321bba8dae", "AQAAAAIAAYagAAAAEFFRB5oFMkUhzXpy5uC+Ttuzs/5x2jWTAyZcXwpLgDs7J33Jbgtnw9dxpRNY2GyVHQ==", "ac131037-e5f3-429f-b6a6-9368afe229d2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "18f3140c-c72b-4d3d-bd53-71c24b1a3c30", "AQAAAAIAAYagAAAAEDPueLQf18pRaOxD4UMxGicRL36KfjjeMZf9r070esFdanArDEwkAK28w/s6Ibq10g==", "137581ee-0ba5-48ef-9021-fff2271df6f2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "409bf59f-72fc-4d7f-8eb8-def1ef67b63d", "AQAAAAIAAYagAAAAENZFhAZuaGGbjhbYompXNqLPYIv1si0da0KGQPNeMh12jiPrfYHguSTo30+QFGIO4A==", "d42f6223-9ad4-46f8-a7f3-d5b2d44fae78" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "caa0f236-6450-46df-98d1-51c2a17516ce", "AQAAAAIAAYagAAAAEKq3lA6jB+0GutRgUbuctnp06lz5thexRgRPdTMR7rULkDuYKCGlxy6UF117ZVX1LA==", "798fc1ea-610a-4738-991c-3baaf8a37750" });

            migrationBuilder.CreateIndex(
                name: "IX_UserWarehouses_RoleId",
                table: "UserWarehouses",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWarehouses_AspNetRoles_RoleId",
                table: "UserWarehouses",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
