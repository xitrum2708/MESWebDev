using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class AddSomeTbl2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PP_calendar_tbl",
                columns: table => new
                {
                    date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_holiday = table.Column<bool>(type: "bit", nullable: true),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PP_calendar_tbl", x => x.date);
                });

            migrationBuilder.CreateTable(
                name: "PP_Para_tbl",
                columns: table => new
                {
                    name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PP_Para_tbl", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "PP_ProdPlan_tbl",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    start_sch_dt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sch_dt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lot_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lot_size = table.Column<int>(type: "int", nullable: false),
                    capa_qty = table.Column<int>(type: "int", nullable: false),
                    bal_qty = table.Column<int>(type: "int", nullable: false),
                    background_color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    border_color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    start = table.Column<DateTime>(type: "datetime2", nullable: true),
                    end = table.Column<DateTime>(type: "datetime2", nullable: true),
                    qty = table.Column<int>(type: "int", nullable: false),
                    working_hour = table.Column<int>(type: "int", nullable: true),
                    is_new = table.Column<bool>(type: "bit", nullable: false),
                    is_fpp = table.Column<bool>(type: "bit", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PP_ProdPlan_tbl", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PP_calendar_tbl");

            migrationBuilder.DropTable(
                name: "PP_Para_tbl");

            migrationBuilder.DropTable(
                name: "PP_ProdPlan_tbl");
        }
    }
}
