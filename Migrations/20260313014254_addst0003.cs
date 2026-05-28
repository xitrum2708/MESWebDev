using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class addst0003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SupplyQty",
                table: "UV_MRP_Data",
                newName: "ReqQty");

            migrationBuilder.RenameColumn(
                name: "NetQty",
                table: "UV_MRP_Data",
                newName: "PlanPOQty");

            migrationBuilder.RenameColumn(
                name: "GrossQty",
                table: "UV_MRP_Data",
                newName: "OHQty");

            migrationBuilder.AddColumn<int>(
                name: "AfterAllLocation",
                table: "UV_MRP_Data",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OBLQty",
                table: "UV_MRP_Data",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlanShipDt",
                table: "UV_MRP_Data",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AfterAllLocation",
                table: "UV_MRP_Data");

            migrationBuilder.DropColumn(
                name: "OBLQty",
                table: "UV_MRP_Data");

            migrationBuilder.DropColumn(
                name: "PlanShipDt",
                table: "UV_MRP_Data");

            migrationBuilder.RenameColumn(
                name: "ReqQty",
                table: "UV_MRP_Data",
                newName: "SupplyQty");

            migrationBuilder.RenameColumn(
                name: "PlanPOQty",
                table: "UV_MRP_Data",
                newName: "NetQty");

            migrationBuilder.RenameColumn(
                name: "OHQty",
                table: "UV_MRP_Data",
                newName: "GrossQty");
        }
    }
}
