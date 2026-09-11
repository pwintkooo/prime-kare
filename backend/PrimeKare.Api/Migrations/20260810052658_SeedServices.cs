using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PrimeKare.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Description", "EstimatedMinutes", "IsActive", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Engine oil and oil filter replacement.", 45, true, "Oil Change", 89.90m },
                    { 2, "Brake inspection, repair and replacement.", 90, true, "Brake Service", 150.00m },
                    { 3, "Computerized engine diagnostics and inspection.", 60, true, "Engine Diagnostics", 120.00m },
                    { 4, "Tyre inspection, replacement and balancing.", 45, true, "Tyre Service", 80.00m },
                    { 5, "Battery testing and replacement service.", 30, true, "Battery Replacement", 180.00m },
                    { 6, "Air conditioning inspection and servicing.", 60, true, "Air Conditioning", 100.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
