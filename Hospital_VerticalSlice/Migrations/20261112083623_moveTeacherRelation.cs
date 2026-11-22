using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_VerticalSlice.Migrations
{
    /// <inheritdoc />
    public partial class moveTeacherRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleTeacher");

            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TeacherId",
                table: "Sessions",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Teachers_TeacherId",
                table: "Sessions",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Teachers_TeacherId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_TeacherId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Sessions");

            migrationBuilder.CreateTable(
                name: "ScheduleTeacher",
                columns: table => new
                {
                    SchedulesId = table.Column<int>(type: "int", nullable: false),
                    TeachersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleTeacher", x => new { x.SchedulesId, x.TeachersId });
                    table.ForeignKey(
                        name: "FK_ScheduleTeacher_Schedules_SchedulesId",
                        column: x => x.SchedulesId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ScheduleTeacher_Teachers_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTeacher_TeachersId",
                table: "ScheduleTeacher",
                column: "TeachersId");
        }
    }
}
