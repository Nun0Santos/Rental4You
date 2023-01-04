﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rental4You.Migrations
{
    public partial class ChangedDelivery : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isReceived",
                table: "deliveries",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isReceived",
                table: "deliveries");

        }
    }
}