using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RefatorandoomodelodeShopCartItemeShopCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedIn",
                table: "ShopCarts");

            migrationBuilder.RenameColumn(
                name: "UpdatedIn",
                table: "ShopCarts",
                newName: "UpdatedOn");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalValue",
                table: "ShopCarts",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedIn",
                table: "ShopCartItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalValue",
                table: "ShopCarts");

            migrationBuilder.DropColumn(
                name: "AddedIn",
                table: "ShopCartItems");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "ShopCarts",
                newName: "UpdatedIn");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedIn",
                table: "ShopCarts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
