using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class addtableabcebdd255 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PcbType",
                table: "UV_SMT_Prod_Plan",
                newName: "PCBType");

            migrationBuilder.RenameColumn(
                name: "PcbPerModel",
                table: "UV_SMT_Prod_Plan",
                newName: "PCBPerModel");

            migrationBuilder.RenameColumn(
                name: "PcbKey",
                table: "UV_SMT_Prod_Plan",
                newName: "PCBKey");

            migrationBuilder.AddColumn<string>(
                name: "UploadedFile",
                table: "UV_SMT_Prod_Plan",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PCB",
                table: "UV_SMT_Lot_PCB",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadedFile",
                table: "UV_SMT_Prod_Plan");

            migrationBuilder.DropColumn(
                name: "PCB",
                table: "UV_SMT_Lot_PCB");

            migrationBuilder.RenameColumn(
                name: "PCBType",
                table: "UV_SMT_Prod_Plan",
                newName: "PcbType");

            migrationBuilder.RenameColumn(
                name: "PCBPerModel",
                table: "UV_SMT_Prod_Plan",
                newName: "PcbPerModel");

            migrationBuilder.RenameColumn(
                name: "PCBKey",
                table: "UV_SMT_Prod_Plan",
                newName: "PcbKey");
        }
    }
}
