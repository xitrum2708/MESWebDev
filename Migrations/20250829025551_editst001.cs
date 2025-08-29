using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class editst001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PE_Manpower_tbl",
                table: "PE_Manpower_tbl");

            migrationBuilder.RenameTable(
                name: "PE_Manpower_tbl",
                newName: "UV_PE_Manpower_tbl");

            migrationBuilder.RenameColumn(
                name: "note",
                table: "UV_PE_Manpower_tbl",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "company",
                table: "UV_PE_Manpower_tbl",
                newName: "Company");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UV_PE_Manpower_tbl",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "upload_file",
                table: "UV_PE_Manpower_tbl",
                newName: "UploadFile");

            migrationBuilder.RenameColumn(
                name: "upload_dt",
                table: "UV_PE_Manpower_tbl",
                newName: "UpdatedModelDt");

            migrationBuilder.RenameColumn(
                name: "upload_by",
                table: "UV_PE_Manpower_tbl",
                newName: "UModel");

            migrationBuilder.RenameColumn(
                name: "updated_dt",
                table: "UV_PE_Manpower_tbl",
                newName: "CreatedDt");

            migrationBuilder.RenameColumn(
                name: "u_model",
                table: "UV_PE_Manpower_tbl",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "smt_headcount",
                table: "UV_PE_Manpower_tbl",
                newName: "SmtHeadcount");

            migrationBuilder.RenameColumn(
                name: "smt_cost",
                table: "UV_PE_Manpower_tbl",
                newName: "SmtCost");

            migrationBuilder.RenameColumn(
                name: "scl_headcount",
                table: "UV_PE_Manpower_tbl",
                newName: "SclHeadcount");

            migrationBuilder.RenameColumn(
                name: "scl_cost",
                table: "UV_PE_Manpower_tbl",
                newName: "SclCost");

            migrationBuilder.RenameColumn(
                name: "insert_headcount",
                table: "UV_PE_Manpower_tbl",
                newName: "InsertHeadcount");

            migrationBuilder.RenameColumn(
                name: "insert_cost",
                table: "UV_PE_Manpower_tbl",
                newName: "InsertCost");

            migrationBuilder.RenameColumn(
                name: "b_model",
                table: "UV_PE_Manpower_tbl",
                newName: "BModel");

            migrationBuilder.RenameColumn(
                name: "average_cost",
                table: "UV_PE_Manpower_tbl",
                newName: "AverageCost");

            migrationBuilder.RenameColumn(
                name: "assy_headcount",
                table: "UV_PE_Manpower_tbl",
                newName: "AssyHeadcount");

            migrationBuilder.RenameColumn(
                name: "assy_cost",
                table: "UV_PE_Manpower_tbl",
                newName: "AssyCost");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UV_PE_Manpower_tbl",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "UV_PE_Manpower_tbl",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDt",
                table: "UV_PE_Manpower_tbl",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UV_PE_Manpower_tbl",
                table: "UV_PE_Manpower_tbl",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UV_PE_Manpower_tbl",
                table: "UV_PE_Manpower_tbl");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UV_PE_Manpower_tbl");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "UV_PE_Manpower_tbl");

            migrationBuilder.DropColumn(
                name: "UpdatedDt",
                table: "UV_PE_Manpower_tbl");

            migrationBuilder.RenameTable(
                name: "UV_PE_Manpower_tbl",
                newName: "PE_Manpower_tbl");

            migrationBuilder.RenameColumn(
                name: "Note",
                table: "PE_Manpower_tbl",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "Company",
                table: "PE_Manpower_tbl",
                newName: "company");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PE_Manpower_tbl",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UploadFile",
                table: "PE_Manpower_tbl",
                newName: "upload_file");

            migrationBuilder.RenameColumn(
                name: "UpdatedModelDt",
                table: "PE_Manpower_tbl",
                newName: "upload_dt");

            migrationBuilder.RenameColumn(
                name: "UModel",
                table: "PE_Manpower_tbl",
                newName: "upload_by");

            migrationBuilder.RenameColumn(
                name: "SmtHeadcount",
                table: "PE_Manpower_tbl",
                newName: "smt_headcount");

            migrationBuilder.RenameColumn(
                name: "SmtCost",
                table: "PE_Manpower_tbl",
                newName: "smt_cost");

            migrationBuilder.RenameColumn(
                name: "SclHeadcount",
                table: "PE_Manpower_tbl",
                newName: "scl_headcount");

            migrationBuilder.RenameColumn(
                name: "SclCost",
                table: "PE_Manpower_tbl",
                newName: "scl_cost");

            migrationBuilder.RenameColumn(
                name: "InsertHeadcount",
                table: "PE_Manpower_tbl",
                newName: "insert_headcount");

            migrationBuilder.RenameColumn(
                name: "InsertCost",
                table: "PE_Manpower_tbl",
                newName: "insert_cost");

            migrationBuilder.RenameColumn(
                name: "CreatedDt",
                table: "PE_Manpower_tbl",
                newName: "updated_dt");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "PE_Manpower_tbl",
                newName: "u_model");

            migrationBuilder.RenameColumn(
                name: "BModel",
                table: "PE_Manpower_tbl",
                newName: "b_model");

            migrationBuilder.RenameColumn(
                name: "AverageCost",
                table: "PE_Manpower_tbl",
                newName: "average_cost");

            migrationBuilder.RenameColumn(
                name: "AssyHeadcount",
                table: "PE_Manpower_tbl",
                newName: "assy_headcount");

            migrationBuilder.RenameColumn(
                name: "AssyCost",
                table: "PE_Manpower_tbl",
                newName: "assy_cost");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PE_Manpower_tbl",
                table: "PE_Manpower_tbl",
                column: "id");
        }
    }
}
