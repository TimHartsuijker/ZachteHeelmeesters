using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMedicalRecordFileForBinaryStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "MedicalRecordFiles");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "MedicalRecordFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MedicalRecordFiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FileContent",
                table: "MedicalRecordFiles",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "MedicalRecordFiles");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "MedicalRecordFiles");

            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "MedicalRecordFiles");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "MedicalRecordFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
