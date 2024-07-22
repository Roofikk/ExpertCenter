using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertCenter.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class SetIndexForArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Products_Article",
                table: "Products",
                column: "Article");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Article",
                table: "Products");
        }
    }
}
