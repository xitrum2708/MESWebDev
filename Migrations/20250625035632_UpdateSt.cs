using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PP_calendar_tbl",
                table: "PP_calendar_tbl");

            migrationBuilder.RenameTable(
                name: "PP_calendar_tbl",
                newName: "PP_Calendar_tbl");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PP_Calendar_tbl",
                table: "PP_Calendar_tbl",
                column: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PP_Calendar_tbl",
                table: "PP_Calendar_tbl");

            migrationBuilder.RenameTable(
                name: "PP_Calendar_tbl",
                newName: "PP_calendar_tbl");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PP_calendar_tbl",
                table: "PP_calendar_tbl",
                column: "date");
        }
    }
}
