using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class abcdef02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "UV_SMT_Prod_Plan_Dtl");

            migrationBuilder.DropColumn(
                name: "CreatedDt",
                table: "UV_SMT_Prod_Plan_Dtl");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "UV_SMT_Prod_Plan_Dtl",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "UV_SMT_Prod_Plan_Dtl");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "UV_SMT_Prod_Plan_Dtl",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDt",
                table: "UV_SMT_Prod_Plan_Dtl",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
