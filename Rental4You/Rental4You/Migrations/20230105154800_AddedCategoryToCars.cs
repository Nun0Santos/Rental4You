using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class AddedCategoryToCars : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "cars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_cars_CategoryId",
                table: "cars",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_cars_Category_CategoryId",
                table: "cars",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cars_Category_CategoryId",
                table: "cars");

            migrationBuilder.DropIndex(
                name: "IX_cars_CategoryId",
                table: "cars");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "cars");
        }
    }
}
