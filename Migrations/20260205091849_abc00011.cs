using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class abc00011 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "UV_SMT_Prod_Plan_Hdr");

            migrationBuilder.DropColumn(
                name: "BorderColor",
                table: "UV_SMT_Prod_Plan_Hdr");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "UV_SMT_Prod_Plan_Hdr");

            migrationBuilder.RenameColumn(
                name: "BorderColor",
                table: "UV_SMT_Prod_Plan_Dtl",
                newName: "borderColor");

            migrationBuilder.RenameColumn(
                name: "BackgroundColor",
                table: "UV_SMT_Prod_Plan_Dtl",
                newName: "backgroundColor");

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanCompletedDt",
                table: "UV_SMT_Prod_Plan_Hdr",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "UV_SMT_Prod_Plan_Dtl",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlanCompletedDt",
                table: "UV_SMT_Prod_Plan_Hdr");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "UV_SMT_Prod_Plan_Dtl");

            migrationBuilder.RenameColumn(
                name: "borderColor",
                table: "UV_SMT_Prod_Plan_Dtl",
                newName: "BorderColor");

            migrationBuilder.RenameColumn(
                name: "backgroundColor",
                table: "UV_SMT_Prod_Plan_Dtl",
                newName: "BackgroundColor");

            migrationBuilder.AddColumn<string>(
                name: "BackgroundColor",
                table: "UV_SMT_Prod_Plan_Hdr",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BorderColor",
                table: "UV_SMT_Prod_Plan_Hdr",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "UV_SMT_Prod_Plan_Hdr",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
