using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class ChangeMaster06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auth_Master_Action",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Master_Action", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_Master_Function",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    EnName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ViName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Controller = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Master_Function", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_Master_Pms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PmsName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Master_Pms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_Master_Role",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Master_Role", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Master_Language",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Culture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Master_Language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Auth_Mapping_Pms_Action",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PmsId = table.Column<int>(type: "int", nullable: false),
                    ActionId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Mapping_Pms_Action", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auth_Mapping_Pms_Action_Auth_Master_Action_ActionId",
                        column: x => x.ActionId,
                        principalTable: "Auth_Master_Action",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Auth_Mapping_Pms_Action_Auth_Master_Pms_PmsId",
                        column: x => x.PmsId,
                        principalTable: "Auth_Master_Pms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auth_Master_User",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LangId = table.Column<int>(type: "int", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Master_User", x => x.Username);
                    table.ForeignKey(
                        name: "FK_Auth_Master_User_Auth_Master_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Auth_Master_Role",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Auth_Master_User_Master_Language_LangId",
                        column: x => x.LangId,
                        principalTable: "Master_Language",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Master_Language_Dic",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LangId = table.Column<int>(type: "int", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Master_Language_Dic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Master_Language_Dic_Master_Language_LangId",
                        column: x => x.LangId,
                        principalTable: "Master_Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auth_Mapping_Role_Func_Pms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    FuncId = table.Column<int>(type: "int", nullable: false),
                    PmsId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Mapping_Role_Func_Pms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auth_Mapping_Role_Func_Pms_Auth_Mapping_Pms_Action_PmsId",
                        column: x => x.PmsId,
                        principalTable: "Auth_Mapping_Pms_Action",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Auth_Mapping_Role_Func_Pms_Auth_Master_Function_FuncId",
                        column: x => x.FuncId,
                        principalTable: "Auth_Master_Function",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Auth_Mapping_Role_Func_Pms_Auth_Master_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Auth_Master_Role",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Auth_Mapping_User_Func_Pms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FuncId = table.Column<int>(type: "int", nullable: false),
                    PmsId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreeatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auth_Mapping_User_Func_Pms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auth_Mapping_User_Func_Pms_Auth_Master_Function_FuncId",
                        column: x => x.FuncId,
                        principalTable: "Auth_Master_Function",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Auth_Mapping_User_Func_Pms_Auth_Master_Pms_PmsId",
                        column: x => x.PmsId,
                        principalTable: "Auth_Master_Pms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Auth_Mapping_User_Func_Pms_Auth_Master_User_UserId",
                        column: x => x.UserId,
                        principalTable: "Auth_Master_User",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auth_Mapping_Pms_Action_ActionId",
                table: "Auth_Mapping_Pms_Action",
                column: "ActionId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_Mapping_Pms_Action_PmsId",
                table: "Auth_Mapping_Pms_Action",
                column: "PmsId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_Mapping_Role_Func_Pms_FuncId",
                table: "Auth_Mapping_Role_Func_Pms",
                column: "FuncId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_Mapping_Role_Func_Pms_PmsId",
                table: "Auth_Mapping_Role_Func_Pms",
                column: "PmsId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_Mapping_Role_Func_Pms_RoleId",
                table: "Auth_Mapping_Role_Func_Pms",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_Mapping_User_Func_Pms_FuncId",
                table: "Auth_Mapping_User_Func_Pms",
                column: "FuncId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_Mapping_User_Func_Pms_PmsId",
                table: "Auth_Mapping_User_Func_Pms",
                column: "PmsId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_Mapping_User_Func_Pms_UserId",
                table: "Auth_Mapping_User_Func_Pms",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_Master_User_LangId",
                table: "Auth_Master_User",
                column: "LangId");

            migrationBuilder.CreateIndex(
                name: "IX_Auth_Master_User_RoleId",
                table: "Auth_Master_User",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Master_Language_Dic_LangId",
                table: "Master_Language_Dic",
                column: "LangId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auth_Mapping_Role_Func_Pms");

            migrationBuilder.DropTable(
                name: "Auth_Mapping_User_Func_Pms");

            migrationBuilder.DropTable(
                name: "Master_Language_Dic");

            migrationBuilder.DropTable(
                name: "Auth_Mapping_Pms_Action");

            migrationBuilder.DropTable(
                name: "Auth_Master_Function");

            migrationBuilder.DropTable(
                name: "Auth_Master_User");

            migrationBuilder.DropTable(
                name: "Auth_Master_Action");

            migrationBuilder.DropTable(
                name: "Auth_Master_Pms");

            migrationBuilder.DropTable(
                name: "Auth_Master_Role");

            migrationBuilder.DropTable(
                name: "Master_Language");
        }
    }
}
