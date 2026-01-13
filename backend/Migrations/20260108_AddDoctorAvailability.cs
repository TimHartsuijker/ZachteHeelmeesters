using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "doctor_availability",
                columns: table => new
                {
                    availability_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    doctor_id = table.Column<int>(type: "int", nullable: false),
                    date_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_available = table.Column<bool>(type: "bit", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctor_availability", x => x.availability_id);
                    table.ForeignKey(
                        name: "FK_doctor_availability_Users_doctor_id",
                        column: x => x.doctor_id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_doctor_availability_doctor_id_date_time",
                table: "doctor_availability",
                columns: new[] { "doctor_id", "date_time" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "doctor_availability");
        }
    }
}
