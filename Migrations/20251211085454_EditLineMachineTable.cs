using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class EditLineMachineTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UV_SMT_Line_UsagePercent");

            migrationBuilder.CreateTable(
                name: "UV_SMT_LineMachineData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MachineCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UsageDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsagePercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_SMT_LineMachineData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UV_SMT_LineMachineData_UV_SMT_Mst_Line_LineCode",
                        column: x => x.LineCode,
                        principalTable: "UV_SMT_Mst_Line",
                        principalColumn: "LineCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UV_SMT_LineMachineData_UV_SMT_Mst_Machine_MachineCode",
                        column: x => x.MachineCode,
                        principalTable: "UV_SMT_Mst_Machine",
                        principalColumn: "MachineCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UV_SMT_LineMachineData_LineCode",
                table: "UV_SMT_LineMachineData",
                column: "LineCode");

            migrationBuilder.CreateIndex(
                name: "IX_UV_SMT_LineMachineData_MachineCode",
                table: "UV_SMT_LineMachineData",
                column: "MachineCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UV_SMT_LineMachineData");

            migrationBuilder.CreateTable(
                name: "UV_SMT_Line_UsagePercent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LineCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MachineCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UsageDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsagePercent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_UV_SMT_Line_UsagePercent_LineCode",
                table: "UV_SMT_Line_UsagePercent",
                column: "LineCode");

            migrationBuilder.CreateIndex(
                name: "IX_UV_SMT_Line_UsagePercent_MachineCode",
                table: "UV_SMT_Line_UsagePercent",
                column: "MachineCode");
        }
    }
}
