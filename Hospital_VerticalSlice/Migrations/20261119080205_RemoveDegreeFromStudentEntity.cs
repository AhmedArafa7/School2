using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_VerticalSlice.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDegreeFromStudentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Degree",
                table: "Students");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Degree",
                table: "Students",
                type: "real",
                nullable: true);
        }
    }
}
