using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class changeTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "border_color",
                table: "PP_ProdPlan_tbl",
                newName: "borderColor");

            migrationBuilder.RenameColumn(
                name: "background_color",
                table: "PP_ProdPlan_tbl",
                newName: "backgroundColor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "borderColor",
                table: "PP_ProdPlan_tbl",
                newName: "border_color");

            migrationBuilder.RenameColumn(
                name: "backgroundColor",
                table: "PP_ProdPlan_tbl",
                newName: "background_color");
        }
    }
}
