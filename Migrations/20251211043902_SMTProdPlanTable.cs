using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class SMTProdPlanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UV_Common_Project_Setting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Property = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_Common_Project_Setting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UV_SMT_Lot_PCB",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lotno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PCBNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_SMT_Lot_PCB", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UV_SMT_Mst_Line",
                columns: table => new
                {
                    LineCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_SMT_Mst_Line", x => x.LineCode);
                });

            migrationBuilder.CreateTable(
                name: "UV_SMT_Mst_Machine",
                columns: table => new
                {
                    MachineCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Capacity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_SMT_Mst_Machine", x => x.MachineCode);
                });

            migrationBuilder.CreateTable(
                name: "UV_SMT_Mst_Shift",
                columns: table => new
                {
                    ShiftCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ShiftName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_SMT_Mst_Shift", x => x.ShiftCode);
                });

            migrationBuilder.CreateTable(
                name: "UV_SMT_Plan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lotno = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: false),
                    Balance = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_SMT_Plan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UV_SMT_Prod_Plan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Market = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartScheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BModel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PcbKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ComponentLot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PcbType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PcbNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Machine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KeyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PcbPerModel = table.Column<int>(type: "int", nullable: false),
                    LotSize = table.Column<int>(type: "int", nullable: false),
                    IssuedQty = table.Column<int>(type: "int", nullable: false),
                    BalanceQty = table.Column<int>(type: "int", nullable: false),
                    TargetPerHour85 = table.Column<int>(type: "int", nullable: false),
                    TargetPerShift = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TimeF = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Warning = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExcessStock = table.Column<int>(type: "int", nullable: true),
                    BackgroundColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BorderColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_SMT_Prod_Plan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UV_SMT_Line_UsagePercent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MachineCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UsageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsagePercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_SMT_Line_UsagePercent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UV_SMT_Line_UsagePercent_UV_SMT_Mst_Line_LineCode",
                        column: x => x.LineCode,
                        principalTable: "UV_SMT_Mst_Line",
                        principalColumn: "LineCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UV_SMT_Line_UsagePercent_UV_SMT_Mst_Machine_MachineCode",
                        column: x => x.MachineCode,
                        principalTable: "UV_SMT_Mst_Machine",
                        principalColumn: "MachineCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UV_SMT_Mst_MachineCondition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MachineCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChipMin = table.Column<int>(type: "int", nullable: false),
                    ChipMax = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_SMT_Mst_MachineCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UV_SMT_Mst_MachineCondition_UV_SMT_Mst_Machine_MachineCode",
                        column: x => x.MachineCode,
                        principalTable: "UV_SMT_Mst_Machine",
                        principalColumn: "MachineCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UV_SMT_Mst_LineCalendar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeekDayOrDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShiftCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_SMT_Mst_LineCalendar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UV_SMT_Mst_LineCalendar_UV_SMT_Mst_Shift_ShiftCode",
                        column: x => x.ShiftCode,
                        principalTable: "UV_SMT_Mst_Shift",
                        principalColumn: "ShiftCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UV_SMT_Line_UsagePercent_LineCode",
                table: "UV_SMT_Line_UsagePercent",
                column: "LineCode");

            migrationBuilder.CreateIndex(
                name: "IX_UV_SMT_Line_UsagePercent_MachineCode",
                table: "UV_SMT_Line_UsagePercent",
                column: "MachineCode");

            migrationBuilder.CreateIndex(
                name: "IX_UV_SMT_Mst_LineCalendar_ShiftCode",
                table: "UV_SMT_Mst_LineCalendar",
                column: "ShiftCode");

            migrationBuilder.CreateIndex(
                name: "IX_UV_SMT_Mst_MachineCondition_MachineCode",
                table: "UV_SMT_Mst_MachineCondition",
                column: "MachineCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UV_Common_Project_Setting");

            migrationBuilder.DropTable(
                name: "UV_SMT_Line_UsagePercent");

            migrationBuilder.DropTable(
                name: "UV_SMT_Lot_PCB");

            migrationBuilder.DropTable(
                name: "UV_SMT_Mst_LineCalendar");

            migrationBuilder.DropTable(
                name: "UV_SMT_Mst_MachineCondition");

            migrationBuilder.DropTable(
                name: "UV_SMT_Plan");

            migrationBuilder.DropTable(
                name: "UV_SMT_Prod_Plan");

            migrationBuilder.DropTable(
                name: "UV_SMT_Mst_Line");

            migrationBuilder.DropTable(
                name: "UV_SMT_Mst_Shift");

            migrationBuilder.DropTable(
                name: "UV_SMT_Mst_Machine");
        }
    }
}
