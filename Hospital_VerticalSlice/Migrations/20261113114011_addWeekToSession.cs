using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_VerticalSlice.Migrations
{
    /// <inheritdoc />
    public partial class addWeekToSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Week",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Week",
                table: "Sessions");
        }
    }
}
