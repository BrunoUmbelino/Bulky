using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bulky.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RefatorandoomodeloProducteseurespectivodataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopCartItems_ShopCarts_ShopCartId",
                table: "ShopCartItems");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ShopCartItems");

            migrationBuilder.DropColumn(
                name: "ListPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PriceAbove100",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PriceUp100",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PriceUp50",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "ShopCartId",
                table: "ShopCartItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Products",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ISBN13",
                table: "Products",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceList",
                table: "Products",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
<<<<<<< HEAD
                name: "Price100More2",
=======
                name: "PriceOver100",
>>>>>>> e3132a7 (.)
                table: "Products",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
<<<<<<< HEAD
                name: "Price50More",
=======
                name: "PriceOver50",
>>>>>>> e3132a7 (.)
                table: "Products",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceStandart",
                table: "Products",
                type: "decimal(6,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
<<<<<<< HEAD
                columns: new[] { "PriceList", "Price100More2", "Price50More", "PriceStandart" },
=======
                columns: new[] { "PriceList", "PriceOver100", "PriceOver50", "PriceStandart" },
>>>>>>> e3132a7 (.)
                values: new object[] { 121.08m, 72.90m, 85.76m, 100.90m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
<<<<<<< HEAD
                columns: new[] { "PriceList", "Price100More2", "Price50More", "PriceStandart" },
=======
                columns: new[] { "PriceList", "PriceOver100", "PriceOver50", "PriceStandart" },
>>>>>>> e3132a7 (.)
                values: new object[] { 390.79m, 234.81m, 276.25m, 325.90m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
<<<<<<< HEAD
                columns: new[] { "PriceList", "Price100More2", "Price50More", "PriceStandart" },
=======
                columns: new[] { "PriceList", "PriceOver100", "PriceOver50", "PriceStandart" },
>>>>>>> e3132a7 (.)
                values: new object[] { 87.48m, 52.67m, 61.96m, 72.90m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
<<<<<<< HEAD
                columns: new[] { "PriceList", "Price100More2", "Price50More", "PriceStandart" },
=======
                columns: new[] { "PriceList", "PriceOver100", "PriceOver50", "PriceStandart" },
>>>>>>> e3132a7 (.)
                values: new object[] { 75.42m, 45.40m, 53.42m, 62.85m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
<<<<<<< HEAD
                columns: new[] { "PriceList", "Price100More2", "Price50More", "PriceStandart" },
=======
                columns: new[] { "PriceList", "PriceOver100", "PriceOver50", "PriceStandart" },
>>>>>>> e3132a7 (.)
                values: new object[] { 51.51m, 31.00m, 36.48m, 42.92m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
<<<<<<< HEAD
                columns: new[] { "PriceList", "Price100More2", "Price50More", "PriceStandart" },
=======
                columns: new[] { "PriceList", "PriceOver100", "PriceOver50", "PriceStandart" },
>>>>>>> e3132a7 (.)
                values: new object[] { 49.86m, 30.01m, 35.31m, 41.55m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
<<<<<<< HEAD
                columns: new[] { "PriceList", "Price100More2", "Price50More", "PriceStandart" },
=======
                columns: new[] { "PriceList", "PriceOver100", "PriceOver50", "PriceStandart" },
>>>>>>> e3132a7 (.)
                values: new object[] { 13.20m, 7.94m, 9.35m, 11.00m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
<<<<<<< HEAD
                columns: new[] { "PriceList", "Price100More2", "Price50More", "PriceStandart" },
=======
                columns: new[] { "PriceList", "PriceOver100", "PriceOver50", "PriceStandart" },
>>>>>>> e3132a7 (.)
                values: new object[] { 86.74m, 52.22m, 61.44m, 72.29m });

            migrationBuilder.AddForeignKey(
                name: "FK_ShopCartItems_ShopCarts_ShopCartId",
                table: "ShopCartItems",
                column: "ShopCartId",
                principalTable: "ShopCarts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShopCartItems_ShopCarts_ShopCartId",
                table: "ShopCartItems");

            migrationBuilder.DropColumn(
                name: "PriceList",
                table: "Products");

            migrationBuilder.DropColumn(
<<<<<<< HEAD
                name: "Price100More2",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price50More",
=======
                name: "PriceOver100",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PriceOver50",
>>>>>>> e3132a7 (.)
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PriceStandart",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "ShopCartId",
                table: "ShopCartItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ShopCartItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "ISBN13",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<double>(
                name: "ListPrice",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PriceAbove100",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PriceUp100",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PriceUp50",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ListPrice", "PriceAbove100", "PriceUp100", "PriceUp50" },
                values: new object[] { 121.08, 72.900000000000006, 85.760000000000005, 100.90000000000001 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ListPrice", "PriceAbove100", "PriceUp100", "PriceUp50" },
                values: new object[] { 390.79000000000002, 234.81, 276.25, 325.89999999999998 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ListPrice", "PriceAbove100", "PriceUp100", "PriceUp50" },
                values: new object[] { 87.480000000000004, 52.670000000000002, 61.960000000000001, 72.900000000000006 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ListPrice", "PriceAbove100", "PriceUp100", "PriceUp50" },
                values: new object[] { 75.420000000000002, 45.399999999999999, 53.420000000000002, 62.850000000000001 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ListPrice", "PriceAbove100", "PriceUp100", "PriceUp50" },
                values: new object[] { 51.509999999999998, 31.0, 36.479999999999997, 42.920000000000002 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ListPrice", "PriceAbove100", "PriceUp100", "PriceUp50" },
                values: new object[] { 49.859999999999999, 30.010000000000002, 35.310000000000002, 41.549999999999997 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "ListPrice", "PriceAbove100", "PriceUp100", "PriceUp50" },
                values: new object[] { 13.199999999999999, 7.9400000000000004, 9.3499999999999996, 11.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "ListPrice", "PriceAbove100", "PriceUp100", "PriceUp50" },
                values: new object[] { 86.739999999999995, 52.219999999999999, 61.439999999999998, 72.290000000000006 });

            migrationBuilder.AddForeignKey(
                name: "FK_ShopCartItems_ShopCarts_ShopCartId",
                table: "ShopCartItems",
                column: "ShopCartId",
                principalTable: "ShopCarts",
                principalColumn: "Id");
        }
    }
}
