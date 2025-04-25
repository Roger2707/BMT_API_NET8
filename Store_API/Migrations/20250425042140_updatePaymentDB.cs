using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store_API.Migrations
{
    /// <inheritdoc />
    public partial class updatePaymentDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "96f7ac4e-eab0-40b5-8bbe-ad31a623618b", "AQAAAAIAAYagAAAAEKmUL+T1zPXXwH+TbeRF9IGQN2ibR7zfXFvcSz0RTzpX2CHdqEtbaAriWVLohsvYCA==", "36dc3eff-69b0-4761-993f-6b94c9361c14" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3a1beb98-acfd-43f7-afbc-f6bb2085c49d", "AQAAAAIAAYagAAAAEE6XAJmAcqdKF/dl+bu8XXHa+BRGt5Pi67I2NwA5bm/7SxEatEg26YCiaKlIqGT5aQ==", "22dfd2a3-aba1-4c7f-a8e9-1942335aabc1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8e0b0596-cdd2-4bc3-b0d4-33555f597d83", "AQAAAAIAAYagAAAAEE2Tq/djxT3TS2Zs5BggljmHpLdZ3W7fkInqqwrkk26UFQVTzARE9oK2o3uAw3uCDw==", "c3d01093-03c5-4ba4-bd3c-b96c85b977c6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5b56249b-e118-49d2-821c-aa8b083e140e", "AQAAAAIAAYagAAAAECHOV9WvFCqViHxJo4Wv4ei+Zn4hBxP72hlHnGmyFTAeY+7F8N+RomC3CVRVBQEydA==", "3f953c1a-f2aa-472d-a226-faaf53c01d05" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8d921130-5946-46fd-a087-c672d506563c", "AQAAAAIAAYagAAAAEB1qhXCEQeTKiyelCfSd0vw9Xb8MMMfcya9GLgWNegFikffYvD2i+G9zAA5XhYRu2g==", "bf5f8778-38df-40d4-9e38-1cac48b35a35" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e758a2c3-c8ee-4e11-a317-d33390568be1", "AQAAAAIAAYagAAAAEEpDSvoh7vufGm7yAiSO6aLJGHI1AQdsf6lBfpqg5KIkMuHZ7tZb+u3zwZqIpjkZOg==", "c0a0ce07-034e-4e3c-8bf1-411d7893e0dc" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "40301ee9-859b-4139-bc82-cea9e73c3590", "AQAAAAIAAYagAAAAEMEVduNgGWOn3l5JLt4J36hY4d7se3KQFWh7jtdsD6AKkawq9Xei0wd1SA3bdQgRAA==", "d3e331f4-b768-450f-88be-c6bf38ce0b67" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0ab54f37-da1e-4a2c-b9a4-9d735693592e", "AQAAAAIAAYagAAAAEFWi1mREQxZ0IQ3rCaAu//WpPXwGyUBtxuCs6L57qVVSquxY5Ak0wVzHkBfIOpmcSA==", "823241ef-46e3-40bc-b10d-64890cefc314" });
        }
    }
}
