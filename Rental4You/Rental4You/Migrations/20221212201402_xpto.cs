using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class xpto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_reservations_returnals_returnalId",
            //    table: "reservations");

            //migrationBuilder.DropTable(
            //    name: "returnals");

            //migrationBuilder.DropIndex(
            //    name: "IX_reservations_returnalId",
            //    table: "reservations");

            //migrationBuilder.DropColumn(
            //    name: "returnalId",
            //    table: "reservations");

            //migrationBuilder.AddColumn<string>(
            //    name: "employeeId",
            //    table: "reservations",
            //    type: "nvarchar(450)",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "carId",
            //    table: "deliveries",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<string>(
            //    name: "employeeId",
            //    table: "deliveries",
            //    type: "nvarchar(450)",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "companyId",
            //    table: "cars",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "BirthDate",
            //    table: "AspNetUsers",
            //    type: "datetime2",
            //    nullable: false,
            //    defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Name",
            //    table: "AspNetUsers",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddColumn<string>(
            //    name: "Surname",
            //    table: "AspNetUsers",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "");

            //migrationBuilder.AddColumn<int>(
            //    name: "TaxNumber",
            //    table: "AspNetUsers",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.CreateTable(
            //    name: "Company",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("SqlServer:Identity", "1, 1"),
            //        Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Company", x => x.Id);
            //    });

            /////

            

            migrationBuilder.CreateIndex(
                name: "IX_reservations_employeeId",
                table: "reservations",
                column: "employeeId");

            migrationBuilder.CreateIndex(
                name: "IX_deliveries_carId",
                table: "deliveries",
                column: "carId");

            migrationBuilder.CreateIndex(
                name: "IX_deliveries_employeeId",
                table: "deliveries",
                column: "employeeId");

            migrationBuilder.CreateIndex(
                name: "IX_cars_companyId",
                table: "cars",
                column: "companyId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Company_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_cars_Company_companyId",
                table: "cars",
                column: "companyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_deliveries_AspNetUsers_employeeId",
                table: "deliveries",
                column: "employeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_deliveries_cars_carId",
                table: "deliveries",
                column: "carId",
                principalTable: "cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_AspNetUsers_employeeId",
                table: "reservations",
                column: "employeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Company_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_cars_Company_companyId",
                table: "cars");

            migrationBuilder.DropForeignKey(
                name: "FK_deliveries_AspNetUsers_employeeId",
                table: "deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_deliveries_cars_carId",
                table: "deliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_reservations_AspNetUsers_employeeId",
                table: "reservations");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropIndex(
                name: "IX_reservations_employeeId",
                table: "reservations");

            migrationBuilder.DropIndex(
                name: "IX_deliveries_carId",
                table: "deliveries");

            migrationBuilder.DropIndex(
                name: "IX_deliveries_employeeId",
                table: "deliveries");

            migrationBuilder.DropIndex(
                name: "IX_cars_companyId",
                table: "cars");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "employeeId",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "carId",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "employeeId",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "companyId",
                table: "cars");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "returnalId",
                table: "reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "returnals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Damage = table.Column<bool>(type: "bit", nullable: false),
                    Km = table.Column<double>(type: "float", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_returnals", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reservations_returnalId",
                table: "reservations",
                column: "returnalId");

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_returnals_returnalId",
                table: "reservations",
                column: "returnalId",
                principalTable: "returnals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
