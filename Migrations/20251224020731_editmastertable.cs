using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class editmastertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MachineName",
                table: "UV_SMT_Mst_Machine",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayColor",
                table: "UV_SMT_Mst_Line",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "UV_SMT_Mst_Line",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LineName",
                table: "UV_SMT_Mst_Line",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MachineName",
                table: "UV_SMT_Mst_Machine");

            migrationBuilder.DropColumn(
                name: "DisplayColor",
                table: "UV_SMT_Mst_Line");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "UV_SMT_Mst_Line");

            migrationBuilder.DropColumn(
                name: "LineName",
                table: "UV_SMT_Mst_Line");
        }
    }
}
