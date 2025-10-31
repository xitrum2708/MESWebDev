using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMaster09 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Auth_Master_Function",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FunctionModelFunctionModel",
                columns: table => new
                {
                    AvailableParentsId = table.Column<int>(type: "int", nullable: false),
                    ChildrenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FunctionModelFunctionModel", x => new { x.AvailableParentsId, x.ChildrenId });
                    table.ForeignKey(
                        name: "FK_FunctionModelFunctionModel_Auth_Master_Function_AvailableParentsId",
                        column: x => x.AvailableParentsId,
                        principalTable: "Auth_Master_Function",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FunctionModelFunctionModel_Auth_Master_Function_ChildrenId",
                        column: x => x.ChildrenId,
                        principalTable: "Auth_Master_Function",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_FunctionModelFunctionModel_ChildrenId",
                table: "FunctionModelFunctionModel",
                column: "ChildrenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FunctionModelFunctionModel");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Auth_Master_Function");
        }
    }
}
