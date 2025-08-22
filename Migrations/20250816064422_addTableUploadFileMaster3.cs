using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MESWebDev.Migrations
{
    /// <inheritdoc />
    public partial class addTableUploadFileMaster3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Master_upload_file_mst",
                table: "Master_upload_file_mst");

            migrationBuilder.RenameTable(
                name: "Master_upload_file_mst",
                newName: "Master_UploadFile_mst");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Master_UploadFile_mst",
                table: "Master_UploadFile_mst",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Master_UploadFile_mst",
                table: "Master_UploadFile_mst");

            migrationBuilder.RenameTable(
                name: "Master_UploadFile_mst",
                newName: "Master_upload_file_mst");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Master_upload_file_mst",
                table: "Master_upload_file_mst",
                column: "id");
        }
    }
}
