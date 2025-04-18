using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store_API.Migrations
{
    /// <inheritdoc />
    public partial class addClientSecretToOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientSecret",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientSecret",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "51471f98-33d3-453f-943c-e2b846fe3150", "AQAAAAIAAYagAAAAEHFiuztwLPAAEj3b5dwrIdbgZflNn/OXYmu97jRfEyETHelJqGHptLJDoIQItRmbRw==", "9f7f5d60-26e7-4de0-97bc-97fb31f7eddd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a605c5bb-75c6-4575-aa41-153e043cd6c9", "AQAAAAIAAYagAAAAEKiQ08T8hSUu8YdIyapW0ueqOd+qGx9THRd2lUgOxrfCdIyK1rKKtDY/mlARcRvIdQ==", "d6292db5-1354-4e4d-8fe0-a2fe9021cda2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cacfe215-d184-48fa-afca-9c99c5c4b43e", "AQAAAAIAAYagAAAAEF4XZbeeYnuSVhp1RF5549zLbJGuu3zcM8UDBJDUMZmfyXCw6RHfnPecQfu4GG8y9Q==", "a4863f73-db81-4e71-b9a8-f13ec1d14bc8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "681f31af-08ca-43e8-b829-8c273f800d2e", "AQAAAAIAAYagAAAAEOTtcu0RlJNfyOG77fsIG/7qPwt/JnjP7UT2FUVwrN8BNXBFfhU3Ze/k/WMDrqJwfQ==", "5a5d892b-3bc6-4d0e-b015-2267402b92a3" });
        }
    }
}
