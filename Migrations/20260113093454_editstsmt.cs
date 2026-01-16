using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class editstsmt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadedFile",
                table: "UV_SMT_Lot_PCB",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadedFile",
                table: "UV_SMT_Lot_PCB");
        }
    }
}
