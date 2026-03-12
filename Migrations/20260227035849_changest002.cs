using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class changest002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RunOrder",
                table: "UV_SMT_Prod_Plan_Dtl",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SetupMinute",
                table: "UV_SMT_Prod_Plan_Dtl",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShiftWindowMinutes",
                table: "UV_SMT_Prod_Plan_Dtl",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "WinStartDt",
                table: "UV_SMT_Prod_Plan_Dtl",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RunOrder",
                table: "UV_SMT_Prod_Plan_Dtl");

            migrationBuilder.DropColumn(
                name: "SetupMinute",
                table: "UV_SMT_Prod_Plan_Dtl");

            migrationBuilder.DropColumn(
                name: "ShiftWindowMinutes",
                table: "UV_SMT_Prod_Plan_Dtl");

            migrationBuilder.DropColumn(
                name: "WinStartDt",
                table: "UV_SMT_Prod_Plan_Dtl");
        }
    }
}
