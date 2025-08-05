using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoServiceCenter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToCustomerAndMechanic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Mechanics",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Mechanics");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Customers");
        }
    }
}
