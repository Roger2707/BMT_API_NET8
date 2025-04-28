using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store_API.Migrations
{
    /// <inheritdoc />
    public partial class createModelShippingOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShippingOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GHNOrderCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingStatus = table.Column<int>(type: "int", nullable: false),
                    ToName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToWard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToDistrict = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToProvince = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    Length = table.Column<int>(type: "int", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    CODAmount = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d451a193-dc63-4b13-b9f7-fea8d8b12b7c", "AQAAAAIAAYagAAAAEFepRJiDRziwCQvwiFhqSrmb/zFWYlNzYHQbIrNm2MzEnSlrkqD8rer56oBNvjzmrA==", "c64a8da9-926d-4a5a-834f-dafb2458e6e6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "950451c2-21f3-45a9-a653-d55c4d7739d4", "AQAAAAIAAYagAAAAEBEiFXf0YO2tfAiFMDOIzxk+gbdKZb4GRFbxYFu/XL2y2ZiY10qNhJX1W0JSFkHVbA==", "6b4d2278-6aeb-4ce5-961b-c0530475ee8d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fe2cc3db-c1e1-4ddf-ab1e-2e8c408dbb94", "AQAAAAIAAYagAAAAEDpQkNk1oHXaC1lZlA5UdlIFIpY+J/htrbElQ/npp6vJUqfULgBr8myyQnIlZ37OSQ==", "520db0a8-a680-4dbf-ac6f-d7ed2e11ae76" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "57cdd321-f6fe-4809-85da-471cc7372ff3", "AQAAAAIAAYagAAAAENgz2v5E9XmysowFdROvchBZPYva6+QGEyJZwfTngITdedMe/AcO1bp5EqbbnwJnJQ==", "dc6d0c3c-77cb-4b42-94e2-1202312783ba" });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingOrders_OrderId",
                table: "ShippingOrders",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingOrders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "38c51ac4-27b7-4cca-93c8-b2f2f2933a1b", "AQAAAAIAAYagAAAAEO3hAL2Uf+SRIjvxBGUyBl4O65gIevlhlhw7JS1rItG1m3IOLy/n1cgyJ23IOISwwg==", "1c7d8b7b-4591-4ed4-a444-eb1131757e45" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "62821565-3d55-4080-ada9-24cb3ef86fbd", "AQAAAAIAAYagAAAAENKPmwPK56G6q3Bfc9p//lma3mtjKRzvZDCqK4XpL2kdkVJaAPdrKSeu0sjwmLaBaw==", "601485fc-b066-43f8-b30f-400ce5a6e963" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "76a2a047-23de-4cfd-8104-2b2dc1b7aa41", "AQAAAAIAAYagAAAAEAsuTj/EOyluAxWR2/cxD5D0PvcHIx3IYlPvI3vyfpqGystF+2bWpjT0pdoZCcGB8A==", "1cf0887a-9b73-4822-87f9-04fb6bab32e8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8c051ff8-af5d-4410-bb96-e8e629b241e9", "AQAAAAIAAYagAAAAEPRE73VtqJXcMtpYRvK/z6/qvr+9AN3biMGXtWzwPpex3u3T6blhhKcqdxuDjVAZvQ==", "89f7f96c-14b1-4b65-aae0-292384df688b" });
        }
    }
}
