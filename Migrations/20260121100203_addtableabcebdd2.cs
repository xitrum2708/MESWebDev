using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class addtableabcebdd2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PcbNo",
                table: "UV_SMT_Prod_Plan",
                newName: "PCBNo");

            migrationBuilder.RenameColumn(
                name: "Machine",
                table: "UV_SMT_Prod_Plan",
                newName: "Model");

            migrationBuilder.RenameColumn(
                name: "ComponentLot",
                table: "UV_SMT_Prod_Plan",
                newName: "MachineCode");

            migrationBuilder.RenameColumn(
                name: "BModel",
                table: "UV_SMT_Prod_Plan",
                newName: "Lotno");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PCBNo",
                table: "UV_SMT_Prod_Plan",
                newName: "PcbNo");

            migrationBuilder.RenameColumn(
                name: "Model",
                table: "UV_SMT_Prod_Plan",
                newName: "Machine");

            migrationBuilder.RenameColumn(
                name: "MachineCode",
                table: "UV_SMT_Prod_Plan",
                newName: "ComponentLot");

            migrationBuilder.RenameColumn(
                name: "Lotno",
                table: "UV_SMT_Prod_Plan",
                newName: "BModel");
        }
    }
}
