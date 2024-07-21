using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertCenter.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class RenameTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntColumns_Product_ProductId",
                table: "IntColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceListColumns_PriceList_PriceListId",
                table: "PriceListColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_PriceList_PriceListId",
                table: "Product");

            migrationBuilder.DropForeignKey(
                name: "FK_StringTextColumns_Product_ProductId",
                table: "StringTextColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_VarCharColumns_Product_ProductId",
                table: "VarCharColumns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceList",
                table: "PriceList");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "Products");

            migrationBuilder.RenameTable(
                name: "PriceList",
                newName: "PriceLists");

            migrationBuilder.RenameIndex(
                name: "IX_Product_PriceListId",
                table: "Products",
                newName: "IX_Products_PriceListId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceLists",
                table: "PriceLists",
                column: "PriceListId");

            migrationBuilder.AddForeignKey(
                name: "FK_IntColumns_Products_ProductId",
                table: "IntColumns",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListColumns_PriceLists_PriceListId",
                table: "PriceListColumns",
                column: "PriceListId",
                principalTable: "PriceLists",
                principalColumn: "PriceListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_PriceLists_PriceListId",
                table: "Products",
                column: "PriceListId",
                principalTable: "PriceLists",
                principalColumn: "PriceListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StringTextColumns_Products_ProductId",
                table: "StringTextColumns",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VarCharColumns_Products_ProductId",
                table: "VarCharColumns",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IntColumns_Products_ProductId",
                table: "IntColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_PriceListColumns_PriceLists_PriceListId",
                table: "PriceListColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_PriceLists_PriceListId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_StringTextColumns_Products_ProductId",
                table: "StringTextColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_VarCharColumns_Products_ProductId",
                table: "VarCharColumns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PriceLists",
                table: "PriceLists");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "Product");

            migrationBuilder.RenameTable(
                name: "PriceLists",
                newName: "PriceList");

            migrationBuilder.RenameIndex(
                name: "IX_Products_PriceListId",
                table: "Product",
                newName: "IX_Product_PriceListId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PriceList",
                table: "PriceList",
                column: "PriceListId");

            migrationBuilder.AddForeignKey(
                name: "FK_IntColumns_Product_ProductId",
                table: "IntColumns",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PriceListColumns_PriceList_PriceListId",
                table: "PriceListColumns",
                column: "PriceListId",
                principalTable: "PriceList",
                principalColumn: "PriceListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_PriceList_PriceListId",
                table: "Product",
                column: "PriceListId",
                principalTable: "PriceList",
                principalColumn: "PriceListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StringTextColumns_Product_ProductId",
                table: "StringTextColumns",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VarCharColumns_Product_ProductId",
                table: "VarCharColumns",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
