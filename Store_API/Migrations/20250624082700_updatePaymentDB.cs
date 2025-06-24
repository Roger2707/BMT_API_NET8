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
            migrationBuilder.AddColumn<string>(
                name: "BasketHash",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c8e67499-ae8e-4fe8-be11-b5e4c5ff6c7b", "AQAAAAIAAYagAAAAEIlApr1n3n4+gcE/EJFmQwpSUO/FBF2XfeapTnNThW9rxLEdDfUipNA/BgsqvcRN/g==", "a800ffb7-67d8-4b01-a65d-3cbc002a3a11" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2f0cd768-7cd2-44b4-99c7-21dc55104bfb", "AQAAAAIAAYagAAAAELYQSbSXVIKRV2QRlEGDb5warBmjmIcCO0SokESicB1dw0M56nQZ2vY9auLcyo3wPQ==", "3a288fe1-5ae3-423e-9d5a-8650741b1fac" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2ea87583-010d-4e95-b598-14a62acfc42f", "AQAAAAIAAYagAAAAEFF+JwHq7C5kzZTmSvzn/hxX459rjNfbp/V8zKApxsuosnONdUk3RatRfu2HiDSk3Q==", "04e97e26-2bc0-441d-8e72-415cf91c59cd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8c5632fc-54c5-4eee-89eb-db6251eaef50", "AQAAAAIAAYagAAAAEEOgHQ2kIfLPalRW04eU5zC07Mt6n5wC0rZijtP2hbMWjtb4NiRK0kjXFow0jIiRMQ==", "c60d5a11-712f-4aab-ba17-b141219181fb" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BasketHash",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "370cbb53-95fb-4444-9af0-dd909b6c3d8a", "AQAAAAIAAYagAAAAEGM3olgHV5Hkb6aYgyulbERTKPvS6om6Slpqxrz/72UIgt0Ja9BAF5UsHMe09g28WQ==", "19b5a321-e4ef-4b58-93cb-e0cbc9dd7546" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4b2e20d1-874f-48a2-b591-298d0d62be24", "AQAAAAIAAYagAAAAEDViiVPMp4T2FmpsW/Kw8PXrRn6hdPtWWPNLj6XfW5FTlKdfAp0gIE3B8wxs6zZcjw==", "4f887993-14f1-4a32-baf5-2e13ecbab236" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "29947c2d-9a59-4b25-8e2c-016c44a936ff", "AQAAAAIAAYagAAAAEE1myrwJpCey3AdJmRuhwoLVf120rb8qt6ytgrGylwJfJG1jS6HkmPr8DOsa9Vw4Tg==", "614e42ed-073e-4d5c-996d-2b106b2a062f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "22584d12-8d68-4049-9669-4096369ed0ef", "AQAAAAIAAYagAAAAEJA+Sy19IuZbySNXwhzCxZ1duw6u025ODlGFOgo/VUF1mQlQNTKhzwA9+u+qOwOZqw==", "443bb6a8-228c-4c91-adf8-4dcf624e4148" });
        }
    }
}
