using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class editOperation01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UV_PE_OperationTimeStudy");

            migrationBuilder.CreateTable(
                name: "UV_PE_TimeStudy_Hdr",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BModel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LotNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PcbName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PcbNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_PE_TimeStudy_Hdr", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UV_PE_TimeStudy_Dtl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: false),
                    OperationKind = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StepNo = table.Column<int>(type: "int", nullable: false),
                    StepContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitQty = table.Column<int>(type: "int", nullable: false),
                    Sumary = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    AllocatedOpr = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_PE_TimeStudy_Dtl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UV_PE_TimeStudy_Dtl_UV_PE_TimeStudy_Hdr_ParentId",
                        column: x => x.ParentId,
                        principalTable: "UV_PE_TimeStudy_Hdr",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UV_PE_TimeStudyStep_Dtl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StepId = table.Column<int>(type: "int", nullable: false),
                    OperationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationDetailName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Time01 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Time02 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Time03 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Time04 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Time05 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_PE_TimeStudyStep_Dtl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UV_PE_TimeStudyStep_Dtl_UV_PE_TimeStudy_Dtl_StepId",
                        column: x => x.StepId,
                        principalTable: "UV_PE_TimeStudy_Dtl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UV_PE_TimeStudy_Dtl_ParentId",
                table: "UV_PE_TimeStudy_Dtl",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UV_PE_TimeStudyStep_Dtl_StepId",
                table: "UV_PE_TimeStudyStep_Dtl",
                column: "StepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UV_PE_TimeStudyStep_Dtl");

            migrationBuilder.DropTable(
                name: "UV_PE_TimeStudy_Dtl");

            migrationBuilder.DropTable(
                name: "UV_PE_TimeStudy_Hdr");

            migrationBuilder.CreateTable(
                name: "UV_PE_OperationTimeStudy",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperationId = table.Column<int>(type: "int", nullable: false),
                    BModel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Customer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LotNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OperationDetailId = table.Column<int>(type: "int", nullable: false),
                    OperationKind = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PcbName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PcbNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Section = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StepNo = table.Column<int>(type: "int", nullable: false),
                    Time01 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Time02 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Time03 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Time04 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Time05 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UploadFile = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_PE_OperationTimeStudy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UV_PE_OperationTimeStudy_Master_Operation_Hdr_OperationId",
                        column: x => x.OperationId,
                        principalTable: "Master_Operation_Hdr",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UV_PE_OperationTimeStudy_OperationId",
                table: "UV_PE_OperationTimeStudy",
                column: "OperationId");
        }
    }
}
