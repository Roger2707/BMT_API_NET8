using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store_API.Migrations
{
    /// <inheritdoc />
    public partial class addEmailInOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "025a6903-8078-4c7d-ac73-a752e19cea89", "AQAAAAIAAYagAAAAEFj6KYRZdJ3jrOgSAUNsGcG8QQy9dod/PG58ogBSRE05C5rOEGsoKidLqOqm6Ff7sQ==", "b32dd84d-0e5c-4992-a9c7-13d379e7d158" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "70989d95-26e9-4945-b1c3-25fddb0fd7ca", "AQAAAAIAAYagAAAAEBvzvTqTPrgRCrLuhZvxL4d33dLo5ptxetKll4e3bZEE/4RAd/ArcJt43Cn6RlSrMg==", "d5348371-6258-4444-9f3a-055ff4a28a2d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "869ffccd-8e2c-4d29-8f30-85a08155628f", "AQAAAAIAAYagAAAAEMXoQedRrIAy35vhpGLyOE/j0ZQDlqTEhXyv4PgQYJRKaAlQ4gca1cTigA1EfleDCA==", "4eb348f4-955a-4c0d-aa49-03de27fbd53b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9d51e593-e5fd-458a-9f15-a88cb4f229e2", "AQAAAAIAAYagAAAAEHrOTKhG1E7NPoEHJh2hO1JAH2RSqHM4pX9rwnGdms8HUghciTtX3+JrxJ/VQAmxUQ==", "55c1a823-5b4d-4eb0-b6db-4dcfe7f1e718" });
        }
    }
}
