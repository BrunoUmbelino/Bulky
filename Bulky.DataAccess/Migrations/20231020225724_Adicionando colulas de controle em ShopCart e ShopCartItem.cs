using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandocolulasdecontroleemShopCarteShopCartItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceOver50",
                table: "Products");

            migrationBuilder.DropColumn(
              name: "PriceOver100",
              table: "Products");

            migrationBuilder.AddColumn<decimal>(
               name: "Price50More",
               table: "Products",
               type: "decimal(6,2)",
               nullable: false,
               defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price100More",
                table: "Products",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedIn",
                table: "ShopCarts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "ShopCartItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 72.90m, 85.76m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 234.81m, 276.25m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 52.67m, 61.96m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 45.40m, 53.42m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 31.00m, 36.48m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 30.01m, 35.31m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 7.94m, 9.35m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 52.22m, 61.44m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "Price50More",
               table: "Products");

            migrationBuilder.DropColumn(
              name: "Price100More",
              table: "Products");

            migrationBuilder.AddColumn<decimal>(
               name: "PriceOver50",
               table: "Products",
               type: "decimal(6,2)",
               nullable: false,
               defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceOver100",
                table: "Products",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.DropColumn(
                name: "AddedIn",
                table: "ShopCarts");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "ShopCartItems");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 0m, 0m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Price100More", "Price50More" },
                values: new object[] { 0m, 0m });
        }
    }
}
