using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class addTableProdCappacity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.CreateTable(
                name: "UV_PE_ProdCapacity_tbl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Customer1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BModel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PCBNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PCBType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SMTPointsTotal = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    COBHC = table.Column<int>(type: "int", nullable: false),
                    COBOutput = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    SMTHC = table.Column<int>(type: "int", nullable: false),
                    SMTOutput = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    InsertHC = table.Column<int>(type: "int", nullable: false),
                    InsertOutput = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    SCHC = table.Column<int>(type: "int", nullable: false),
                    SCOutput = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    AssyHC = table.Column<int>(type: "int", nullable: false),
                    AssyOutput = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    MicHC = table.Column<int>(type: "int", nullable: false),
                    MicOutput = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    OtherHC = table.Column<int>(type: "int", nullable: false),
                    OtherOutput = table.Column<int>(type: "int", nullable: false),
                    SMTTop = table.Column<int>(type: "int", nullable: false),
                    SMTBottom = table.Column<int>(type: "int", nullable: false),
                    SMTTotal = table.Column<int>(type: "int", nullable: false),
                    Insert = table.Column<int>(type: "int", nullable: false),
                    PcsPerSheet = table.Column<int>(type: "int", nullable: false),
                    UploadFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UV_PE_ProdCapacity_tbl", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "UV_PE_ProdCapacity_tbl");
        }
    }
}
