using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store_API.Migrations
{
    /// <inheritdoc />
    public partial class addRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Products_ProductId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_ProductId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductDetailId",
                table: "Ratings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5ab11aa0-3e97-4f84-b4ff-ee08432badb7", "AQAAAAIAAYagAAAAECDXw0OUINM5p+kUCiZtFSiSTWDcRZrNC5KOzb2SNhhfVOQoCQfGE1FJ6beTIj9H/g==", "cbe23b87-265a-461a-83f8-f1e10084db5a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "58dc9edb-c496-49b3-a2e5-298a33b6bd26", "AQAAAAIAAYagAAAAEHbTb9VVxn9aIr7Irefwa706ZVCR4JRGlRVf1Gg3+1xW2iTv5coW6ecJxpv4tmBKhw==", "05be238d-4992-410e-a890-2035ed91e493" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2f452433-a910-4788-90c6-b2b691c01c19", "AQAAAAIAAYagAAAAEAusFnPYL8BxRyyMciZ0AJNtQh2nS2HcKqTQ4LKNmvULKXsOKSyUukXNBNj+MYSFzA==", "298dfa3c-5385-4768-bafb-71357f5a9fef" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bd4bd4b4-540e-4d85-b8ef-97c9039006da", "AQAAAAIAAYagAAAAELp7LsPVP/1T5MdaORThQC8VWoHIV9gtD5xqu0OErhkfT+eIVywj6qBBerA6Vt/F2Q==", "a4532abe-4b82-4bd2-8a39-ca7e17b2bb15" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductDetailId",
                table: "Ratings");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "77262f22-2ecd-4ee9-94d7-af7e143d96ff", "AQAAAAIAAYagAAAAEOG3HJDD1AjbKySNV+hMTeS0EKvwBocaMiSgr2CWI+zlOF4RlkDF2PwGyPJa7uTojg==", "7939fbef-1aa5-4931-8c5f-200a52f8418b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "18347a5c-704a-4c58-a407-72a063be7440", "AQAAAAIAAYagAAAAEF0d/ZaXtRojKhwd0Ifm4HSBIfljKbSeQI1pstjLh7TReiEJW8AvR6b6EARzy6vPBw==", "667001b4-8a42-4b1f-80af-e6d20290783d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b831d386-1fd2-4d4b-a095-e1a7867482f4", "AQAAAAIAAYagAAAAEChrz5cFx70SXRXUsPSvwnpW+8kqEFvmcRKJMkQM7Xlv0FFOryTSQxYBsIi5U7T0Ng==", "556a404a-4cc9-4b1b-b1a3-0834c25c086e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c3527dba-3635-4f06-924e-b8c3a63b2e24", "AQAAAAIAAYagAAAAEDsGhOshqEsSGY4+OmAlTAytNDifaop00Nq4Y78jyLNAfIU2gK3n3vgohA1rVB1WsA==", "57716f29-fa95-4892-ab02-445633ea1e49" });

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ProductId",
                table: "Ratings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_UserId",
                table: "Ratings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Products_ProductId",
                table: "Ratings",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
