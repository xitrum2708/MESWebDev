using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class changest006 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Qty",
                table: "UV_MRP_SPO",
                newName: "SPOQty");

            migrationBuilder.RenameColumn(
                name: "QtySet",
                table: "UV_MRP_BOM",
                newName: "BOMQty");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SPOQty",
                table: "UV_MRP_SPO",
                newName: "Qty");

            migrationBuilder.RenameColumn(
                name: "BOMQty",
                table: "UV_MRP_BOM",
                newName: "QtySet");
        }
    }
}
