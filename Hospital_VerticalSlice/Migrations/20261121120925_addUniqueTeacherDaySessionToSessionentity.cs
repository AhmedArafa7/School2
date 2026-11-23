using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace School_VerticalSlice.Migrations
{
    /// <inheritdoc />
    public partial class addUniqueTeacherDaySessionToSessionentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Day_SessionNumber_TeacherId",
                table: "Sessions",
                columns: new[] { "Day", "SessionNumber", "TeacherId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sessions_Day_SessionNumber_TeacherId",
                table: "Sessions");
        }
    }
}
