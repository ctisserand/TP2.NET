using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gauniv.WebServer.Migrations
{
    /// <inheritdoc />
    public partial class AddGameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "price",
                table: "Games",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "payload",
                table: "Games",
                newName: "Payload");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Games",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "Payload",
                table: "Games",
                newName: "payload");
        }
    }
}
