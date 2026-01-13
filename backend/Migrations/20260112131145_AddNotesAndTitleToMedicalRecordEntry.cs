using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddNotesAndTitleToMedicalRecordEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecordEntries_Appointments_AppointmentId",
                table: "MedicalRecordEntries");

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "MedicalRecordEntries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "MedicalRecordEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "MedicalRecordEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "MedicalRecordEntries",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecordEntries_Appointments_AppointmentId",
                table: "MedicalRecordEntries",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecordEntries_Appointments_AppointmentId",
                table: "MedicalRecordEntries");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "MedicalRecordEntries");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "MedicalRecordEntries");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "MedicalRecordEntries");

            migrationBuilder.AlterColumn<int>(
                name: "AppointmentId",
                table: "MedicalRecordEntries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecordEntries_Appointments_AppointmentId",
                table: "MedicalRecordEntries",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
