using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class updatest004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitQty",
                table: "UV_PE_OperationTimeStudy",
                newName: "StepNo");

            migrationBuilder.RenameColumn(
                name: "StepNumber",
                table: "UV_PE_OperationTimeStudy",
                newName: "Qty");

            migrationBuilder.RenameColumn(
                name: "BcpNumber",
                table: "UV_PE_OperationTimeStudy",
                newName: "PcbNo");

            migrationBuilder.RenameColumn(
                name: "BcpName",
                table: "UV_PE_OperationTimeStudy",
                newName: "PcbName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StepNo",
                table: "UV_PE_OperationTimeStudy",
                newName: "UnitQty");

            migrationBuilder.RenameColumn(
                name: "Qty",
                table: "UV_PE_OperationTimeStudy",
                newName: "StepNumber");

            migrationBuilder.RenameColumn(
                name: "PcbNo",
                table: "UV_PE_OperationTimeStudy",
                newName: "BcpNumber");

            migrationBuilder.RenameColumn(
                name: "PcbName",
                table: "UV_PE_OperationTimeStudy",
                newName: "BcpName");
        }
    }
}
