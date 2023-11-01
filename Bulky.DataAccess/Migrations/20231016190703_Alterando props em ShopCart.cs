using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AlterandopropsemShopCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopCarts_AspNetUsers_ApplicationUserId",
                table: "ShopCarts");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "ShopCartItems",
                newName: "Quantity");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ShopCarts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedIn",
                table: "ShopCarts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedIn",
                table: "ShopCarts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ShopCartItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopCarts_AspNetUsers_ApplicationUserId",
                table: "ShopCarts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopCarts_AspNetUsers_ApplicationUserId",
                table: "ShopCarts");

            migrationBuilder.DropColumn(
                name: "CreatedIn",
                table: "ShopCarts");

            migrationBuilder.DropColumn(
                name: "UpdatedIn",
                table: "ShopCarts");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "ShopCartItems",
                newName: "Count");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ShopCarts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "ShopCartItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopCarts_AspNetUsers_ApplicationUserId",
                table: "ShopCarts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
