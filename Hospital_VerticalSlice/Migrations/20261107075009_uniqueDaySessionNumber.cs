using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_VerticalSlice.Migrations
{
    /// <inheritdoc />
    public partial class uniqueDaySessionNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Class_ClassId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Teachers_TeacherId",
                table: "Sessions");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Day_SessionNumber",
                table: "Sessions",
                columns: new[] { "Day", "SessionNumber" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Class_ClassId",
                table: "Sessions",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Teachers_TeacherId",
                table: "Sessions",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Class_ClassId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Teachers_TeacherId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_Day_SessionNumber",
                table: "Sessions");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Class_ClassId",
                table: "Sessions",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Teachers_TeacherId",
                table: "Sessions",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
