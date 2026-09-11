using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimeKare.Api.Migrations
{
    /// <inheritdoc />
    public partial class UserSchemaFixError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Statis",
                table: "Users",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Users",
                newName: "Statis");
        }
    }
}
