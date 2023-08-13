using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mage.Migrations
{
    public partial class addShipping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Link",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "CartItems");

            migrationBuilder.AddColumn<int>(
                name: "ShippingCost",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingCost",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
