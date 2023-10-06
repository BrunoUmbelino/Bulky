using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AlterandomodelodeCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPriceItem",
                table: "shopCartItems");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "ShopCartItems",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "ShopCartItems");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "ShopCartItems",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
