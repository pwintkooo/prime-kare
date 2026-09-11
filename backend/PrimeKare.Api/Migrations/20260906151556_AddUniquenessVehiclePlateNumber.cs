using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimeKare.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUniquenessVehiclePlateNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_PlateNumber",
                table: "Vehicles",
                column: "PlateNumber",
                unique: true,
                filter: "\"IsDeleted\" = false AND \"Status\" = 'active'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Vehicles_PlateNumber",
                table: "Vehicles");
        }
    }
}
