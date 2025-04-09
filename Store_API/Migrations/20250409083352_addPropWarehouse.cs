using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Store_API.Migrations
{
    /// <inheritdoc />
    public partial class addPropWarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuperAdminOnly",
                table: "Warehouses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a5bf1ff0-7685-4ef2-ac80-9a342be4b8b6", "AQAAAAIAAYagAAAAENn2iPkJ6lUSqWPv1UjebKBOzJFweJGniccBF3NRkxvHYLbBYXWu3ms1YPcvbtxYJg==", "4cba8bc8-1afd-4a28-89b4-703361422ede" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c3c81245-0a92-472c-95f9-6f49568dbdd7", "AQAAAAIAAYagAAAAEImsXkb2aAc7O80XyxqK7LDWNjeOSdnpXxpSBod5OMYhHN/gKpu9j+aaK7GGNd/CCw==", "612ba8d4-380b-4770-b1d6-332ee3aab354" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a5491091-b970-4c7e-9215-1c60c2190f89", "AQAAAAIAAYagAAAAEI4xSWJV5/Y2IPPNkrjvC+4xs802HvsIUQIWUHdvD8y/lc1XGQmlqWddVHHxWcaJbA==", "ceebb39c-0725-4de4-9e02-c9507ff35743" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "34073544-fd04-4207-ad1c-4ac32a073039", "AQAAAAIAAYagAAAAEA8dkiqm3quqR3na1gKpYo4Iyoit17NejnXbXktkcMkKNykf8osT7LAPQynqGwNVoA==", "378ff0b8-e950-4432-b60c-e7307e78f685" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuperAdminOnly",
                table: "Warehouses");

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
    }
}
