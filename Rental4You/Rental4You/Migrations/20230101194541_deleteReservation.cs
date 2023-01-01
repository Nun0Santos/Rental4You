using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class deleteReservation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_cars_Company_companyId",
            //    table: "cars");

            migrationBuilder.DropTable(
                name: "reservations");

            //migrationBuilder.RenameColumn(
            //    name: "companyId",
            //    table: "cars",
            //    newName: "CompanyId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_cars_companyId",
            //    table: "cars",
            //    newName: "IX_cars_CompanyId");

            //migrationBuilder.AddColumn<int>(
            //    name: "Rating",
            //    table: "Company",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_cars_Company_CompanyId",
                table: "cars",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_cars_Company_CompanyId",
            //    table: "cars");

            //migrationBuilder.DropColumn(
            //    name: "Rating",
            //    table: "Company");

            //migrationBuilder.RenameColumn(
            //    name: "CompanyId",
            //    table: "cars",
            //    newName: "companyId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_cars_CompanyId",
            //    table: "cars",
            //    newName: "IX_cars_companyId");

            //migrationBuilder.CreateTable(
            //    name: "reservations",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        carId = table.Column<int>(type: "int", nullable: false),
            //        deliveryId = table.Column<int>(type: "int", nullable: false),
            //        employeeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
            //        End = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        Start = table.Column<DateTime>(type: "datetime2", nullable: false),
            //        price = table.Column<float>(type: "real", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_reservations", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_reservations_AspNetUsers_employeeId",
            //            column: x => x.employeeId,
            //            principalTable: "AspNetUsers",
            //            principalColumn: "Id");
            //        table.ForeignKey(
            //            name: "FK_reservations_cars_carId",
            //            column: x => x.carId,
            //            principalTable: "cars",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //        table.ForeignKey(
            //            name: "FK_reservations_deliveries_deliveryId",
            //            column: x => x.deliveryId,
            //            principalTable: "deliveries",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_reservations_carId",
            //    table: "reservations",
            //    column: "carId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_reservations_deliveryId",
            //    table: "reservations",
            //    column: "deliveryId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_reservations_employeeId",
            //    table: "reservations",
            //    column: "employeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_cars_Company_companyId",
                table: "cars",
                column: "companyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
