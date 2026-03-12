using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class abcdef01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "UV_SMT_Prod_Plan",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDt",
                table: "UV_SMT_Prod_Plan",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "UV_SMT_Lot_PCB",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDt",
                table: "UV_SMT_Lot_PCB",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UV_SMT_Prod_Plan_Hdr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Market = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PCBKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lotno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PCBType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PCBNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KeyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PCBPerModel = table.Column<int>(type: "int", nullable: false),
                    LotSize = table.Column<int>(type: "int", nullable: false),
                    IssuedQty = table.Column<int>(type: "int", nullable: false),
                    BalanceQty = table.Column<int>(type: "int", nullable: false),
                    TargetPerHour85 = table.Column<int>(type: "int", nullable: false),
                    TargetPerShift = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TimeF = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Warning = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcessStock = table.Column<int>(type: "int", nullable: true),
                    UVNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ETPCB = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BackgroundColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BorderColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_SMT_Prod_Plan_Hdr", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UV_SMT_Prod_Plan_Dtl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HeaderId = table.Column<int>(type: "int", nullable: false),
                    Lotno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PCBKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LineCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    TimeTotal = table.Column<int>(type: "int", nullable: false),
                    BackgroundColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BorderColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_SMT_Prod_Plan_Dtl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UV_SMT_Prod_Plan_Dtl_UV_SMT_Prod_Plan_Hdr_HeaderId",
                        column: x => x.HeaderId,
                        principalTable: "UV_SMT_Prod_Plan_Hdr",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UV_SMT_Prod_Plan_Dtl_HeaderId",
                table: "UV_SMT_Prod_Plan_Dtl",
                column: "HeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UV_SMT_Prod_Plan_Dtl");

            migrationBuilder.DropTable(
                name: "UV_SMT_Prod_Plan_Hdr");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "UV_SMT_Prod_Plan");

            migrationBuilder.DropColumn(
                name: "UpdatedDt",
                table: "UV_SMT_Prod_Plan");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "UV_SMT_Lot_PCB");

            migrationBuilder.DropColumn(
                name: "UpdatedDt",
                table: "UV_SMT_Lot_PCB");
        }
    }
}
