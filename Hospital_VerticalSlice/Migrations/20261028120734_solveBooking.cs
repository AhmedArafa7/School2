using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_VerticalSlice.Migrations
{
    /// <inheritdoc />
    public partial class solveBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvirageTime",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DayAppointment",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvirageTime",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "DayAppointment",
                table: "Appointments");
        }
    }
}
