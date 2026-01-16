using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class abcdef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UV_SMT_Mst_LineCalendar_UV_SMT_Mst_Shift_ShiftCode",
                table: "UV_SMT_Mst_LineCalendar");

            migrationBuilder.DropIndex(
                name: "IX_UV_SMT_Mst_LineCalendar_ShiftCode",
                table: "UV_SMT_Mst_LineCalendar");

            migrationBuilder.AlterColumn<string>(
                name: "ShiftCode",
                table: "UV_SMT_Mst_LineCalendar",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShiftCode",
                table: "UV_SMT_Mst_LineCalendar",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UV_SMT_Mst_LineCalendar_ShiftCode",
                table: "UV_SMT_Mst_LineCalendar",
                column: "ShiftCode");

            migrationBuilder.AddForeignKey(
                name: "FK_UV_SMT_Mst_LineCalendar_UV_SMT_Mst_Shift_ShiftCode",
                table: "UV_SMT_Mst_LineCalendar",
                column: "ShiftCode",
                principalTable: "UV_SMT_Mst_Shift",
                principalColumn: "ShiftCode",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
