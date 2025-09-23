using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DapperrShopPractice.Migrations
{
    /// <inheritdoc />
    public partial class WhyItsDontWork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__OrderProd__Order__4222D4EF",
                table: "OrderProducts");

            migrationBuilder.DropForeignKey(
                name: "FK__OrderProd__Produ__4316F928",
                table: "OrderProducts");

            migrationBuilder.AddForeignKey(
                name: "FK__OrderProd__Order__4222D4EF",
                table: "OrderProducts",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK__OrderProd__Produ__4316F928",
                table: "OrderProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__OrderProd__Order__4222D4EF",
                table: "OrderProducts");

            migrationBuilder.DropForeignKey(
                name: "FK__OrderProd__Produ__4316F928",
                table: "OrderProducts");

            migrationBuilder.AddForeignKey(
                name: "FK__OrderProd__Order__4222D4EF",
                table: "OrderProducts",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__OrderProd__Produ__4316F928",
                table: "OrderProducts",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
