using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMaster10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auth_Mapping_Role_Func_Pms_Auth_Mapping_Pms_Action_PmsId",
                table: "Auth_Mapping_Role_Func_Pms");

            migrationBuilder.DropTable(
                name: "FunctionModelFunctionModel");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Auth_Master_Function");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_Master_Function_ParentId",
                table: "Auth_Master_Function",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auth_Mapping_Role_Func_Pms_Auth_Master_Pms_PmsId",
                table: "Auth_Mapping_Role_Func_Pms",
                column: "PmsId",
                principalTable: "Auth_Master_Pms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Auth_Master_Function_Auth_Master_Function_ParentId",
                table: "Auth_Master_Function",
                column: "ParentId",
                principalTable: "Auth_Master_Function",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auth_Mapping_Role_Func_Pms_Auth_Master_Pms_PmsId",
                table: "Auth_Mapping_Role_Func_Pms");

            migrationBuilder.DropForeignKey(
                name: "FK_Auth_Master_Function_Auth_Master_Function_ParentId",
                table: "Auth_Master_Function");

            migrationBuilder.DropIndex(
                name: "IX_Auth_Master_Function_ParentId",
                table: "Auth_Master_Function");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Auth_Mapping_Role_Func_Pms_Auth_Mapping_Pms_Action_PmsId",
                table: "Auth_Mapping_Role_Func_Pms",
                column: "PmsId",
                principalTable: "Auth_Mapping_Pms_Action",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
