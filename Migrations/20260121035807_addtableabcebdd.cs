using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class addtableabcebdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ETPCB",
                table: "UV_SMT_Prod_Plan",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UVNote",
                table: "UV_SMT_Prod_Plan",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ETPCB",
                table: "UV_SMT_Prod_Plan");

            migrationBuilder.DropColumn(
                name: "UVNote",
                table: "UV_SMT_Prod_Plan");
        }
    }
}
