using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpertCenter.DataContext.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColumnTypeId",
                table: "Columns",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ColumnTypes",
                columns: table => new
                {
                    ColumnTypeId = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayColumnName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnTypes", x => x.ColumnTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Columns_ColumnTypeId",
                table: "Columns",
                column: "ColumnTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Columns_ColumnTypes_ColumnTypeId",
                table: "Columns",
                column: "ColumnTypeId",
                principalTable: "ColumnTypes",
                principalColumn: "ColumnTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Columns_ColumnTypes_ColumnTypeId",
                table: "Columns");

            migrationBuilder.DropTable(
                name: "ColumnTypes");

            migrationBuilder.DropIndex(
                name: "IX_Columns_ColumnTypeId",
                table: "Columns");

            migrationBuilder.DropColumn(
                name: "ColumnTypeId",
                table: "Columns");
        }
    }
}
