using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class addTablePEmanpower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "PE_Manpower_tbl",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    u_model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    b_model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    smt_headcount = table.Column<int>(type: "int", nullable: false),
                    insert_headcount = table.Column<int>(type: "int", nullable: false),
                    assy_headcount = table.Column<int>(type: "int", nullable: false),
                    scl_headcount = table.Column<int>(type: "int", nullable: false),
                    smt_cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    insert_cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    scl_cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    assy_cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    average_cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    upload_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_dt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    upload_dt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    upload_by = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PE_Manpower_tbl", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PE_Manpower_tbl");
        }
    }
}
