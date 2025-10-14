using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class TimeStudyNew001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OperationName",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                newName: "ProcessName");

            migrationBuilder.AlterColumn<decimal>(
                name: "TimeAvg",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "Time05",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "Time04",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "Time03",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "Time02",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "Time01",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AddColumn<int>(
                name: "ProcessQty",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "BottleNeckProcess",
                table: "UV_PE_TimeStudyNew_Hdr",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrepare",
                table: "UV_PE_TimeStudyNew_Hdr",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "LineBalance",
                table: "UV_PE_TimeStudyNew_Hdr",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ModelCat",
                table: "UV_PE_TimeStudyNew_Hdr",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OperatorQty",
                table: "UV_PE_TimeStudyNew_Hdr",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OutputTarget",
                table: "UV_PE_TimeStudyNew_Hdr",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PitchTime",
                table: "UV_PE_TimeStudyNew_Hdr",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TimeTotal",
                table: "UV_PE_TimeStudyNew_Hdr",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ProcessTime",
                table: "UV_PE_TimeStudyNew_Dtl",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SetTime",
                table: "UV_PE_TimeStudyNew_Dtl",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TargetQty",
                table: "UV_PE_TimeStudyNew_Dtl",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessQty",
                table: "UV_PE_TimeStudyNewStep_Dtl");

            migrationBuilder.DropColumn(
                name: "BottleNeckProcess",
                table: "UV_PE_TimeStudyNew_Hdr");

            migrationBuilder.DropColumn(
                name: "IsPrepare",
                table: "UV_PE_TimeStudyNew_Hdr");

            migrationBuilder.DropColumn(
                name: "LineBalance",
                table: "UV_PE_TimeStudyNew_Hdr");

            migrationBuilder.DropColumn(
                name: "ModelCat",
                table: "UV_PE_TimeStudyNew_Hdr");

            migrationBuilder.DropColumn(
                name: "OperatorQty",
                table: "UV_PE_TimeStudyNew_Hdr");

            migrationBuilder.DropColumn(
                name: "OutputTarget",
                table: "UV_PE_TimeStudyNew_Hdr");

            migrationBuilder.DropColumn(
                name: "PitchTime",
                table: "UV_PE_TimeStudyNew_Hdr");

            migrationBuilder.DropColumn(
                name: "TimeTotal",
                table: "UV_PE_TimeStudyNew_Hdr");

            migrationBuilder.DropColumn(
                name: "ProcessTime",
                table: "UV_PE_TimeStudyNew_Dtl");

            migrationBuilder.DropColumn(
                name: "SetTime",
                table: "UV_PE_TimeStudyNew_Dtl");

            migrationBuilder.DropColumn(
                name: "TargetQty",
                table: "UV_PE_TimeStudyNew_Dtl");

            migrationBuilder.RenameColumn(
                name: "ProcessName",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                newName: "OperationName");

            migrationBuilder.AlterColumn<decimal>(
                name: "TimeAvg",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Time05",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Time04",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Time03",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Time02",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Time01",
                table: "UV_PE_TimeStudyNewStep_Dtl",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);
        }
    }
}
