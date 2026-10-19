using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class editTypeofDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SMTPointsTotal",
                table: "UV_PE_ProdCapacity_tbl",
                type: "decimal(18,4)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<int>(
                name: "SMTOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<int>(
                name: "SCOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<int>(
                name: "OtherOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "MicOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<int>(
                name: "InsertOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<int>(
                name: "COBOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");

            migrationBuilder.AlterColumn<int>(
                name: "AssyOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,6)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SMTPointsTotal",
                table: "UV_PE_ProdCapacity_tbl",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "SMTOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "SCOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "OtherOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "MicOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "InsertOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "COBOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "AssyOutput",
                table: "UV_PE_ProdCapacity_tbl",
                type: "decimal(18,6)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
