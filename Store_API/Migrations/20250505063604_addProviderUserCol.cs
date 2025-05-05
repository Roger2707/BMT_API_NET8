using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store_API.Migrations
{
    /// <inheritdoc />
    public partial class addProviderUserCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "Provider", "SecurityStamp" },
                values: new object[] { "370cbb53-95fb-4444-9af0-dd909b6c3d8a", "AQAAAAIAAYagAAAAEGM3olgHV5Hkb6aYgyulbERTKPvS6om6Slpqxrz/72UIgt0Ja9BAF5UsHMe09g28WQ==", "System", "19b5a321-e4ef-4b58-93cb-e0cbc9dd7546" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "Provider", "SecurityStamp" },
                values: new object[] { "4b2e20d1-874f-48a2-b591-298d0d62be24", "AQAAAAIAAYagAAAAEDViiVPMp4T2FmpsW/Kw8PXrRn6hdPtWWPNLj6XfW5FTlKdfAp0gIE3B8wxs6zZcjw==", "System", "4f887993-14f1-4a32-baf5-2e13ecbab236" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "Provider", "SecurityStamp" },
                values: new object[] { "29947c2d-9a59-4b25-8e2c-016c44a936ff", "AQAAAAIAAYagAAAAEE1myrwJpCey3AdJmRuhwoLVf120rb8qt6ytgrGylwJfJG1jS6HkmPr8DOsa9Vw4Tg==", "System", "614e42ed-073e-4d5c-996d-2b106b2a062f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "Provider", "SecurityStamp" },
                values: new object[] { "22584d12-8d68-4049-9669-4096369ed0ef", "AQAAAAIAAYagAAAAEJA+Sy19IuZbySNXwhzCxZ1duw6u025ODlGFOgo/VUF1mQlQNTKhzwA9+u+qOwOZqw==", "System", "443bb6a8-228c-4c91-adf8-4dcf624e4148" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                table: "AspNetUsers");

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
        }
    }
}
