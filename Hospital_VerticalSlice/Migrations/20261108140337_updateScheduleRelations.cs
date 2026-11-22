using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hospital_VerticalSlice.Migrations
{
    /// <inheritdoc />
    public partial class updateScheduleRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_Sessions_SessionId",
                table: "Attendance");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_Students_StudentId",
                table: "Attendance");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Class_ClassId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Class_ClassId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Teachers_TeacherId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Class_ClassId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Schedules_ScheduleId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_ScheduleId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_ClassId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_Day_SessionNumber",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_TeacherId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_ClassId",
                table: "Schedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Class",
                table: "Class");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendance",
                table: "Attendance");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Schedules");

            migrationBuilder.RenameTable(
                name: "Class",
                newName: "Classes");

            migrationBuilder.RenameTable(
                name: "Attendance",
                newName: "Attendances");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Attendances",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Attendance_StudentId",
                table: "Attendances",
                newName: "IX_Attendances_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendance_SessionId",
                table: "Attendances",
                newName: "IX_Attendances_SessionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classes",
                table: "Classes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ClassSchedule",
                columns: table => new
                {
                    ClassesId = table.Column<int>(type: "int", nullable: false),
                    schedulesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSchedule", x => new { x.ClassesId, x.schedulesId });
                    table.ForeignKey(
                        name: "FK_ClassSchedule_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassSchedule_Schedules_schedulesId",
                        column: x => x.schedulesId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Sessions_Day_SessionNumber_ScheduleId",
                table: "Sessions",
                columns: new[] { "Day", "SessionNumber", "ScheduleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedule_schedulesId",
                table: "ClassSchedule",
                column: "schedulesId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleTeacher_TeachersId",
                table: "ScheduleTeacher",
                column: "TeachersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Sessions_SessionId",
                table: "Attendances",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Students_StudentId",
                table: "Attendances",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Classes_ClassId",
                table: "Students",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Sessions_SessionId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Students_StudentId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Classes_ClassId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "ClassSchedule");

            migrationBuilder.DropTable(
                name: "ScheduleTeacher");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_Day_SessionNumber_ScheduleId",
                table: "Sessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classes",
                table: "Classes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attendances",
                table: "Attendances");

            migrationBuilder.RenameTable(
                name: "Classes",
                newName: "Class");

            migrationBuilder.RenameTable(
                name: "Attendances",
                newName: "Attendance");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Attendance",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendance",
                newName: "IX_Attendance_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_SessionId",
                table: "Attendance",
                newName: "IX_Attendance_SessionId");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Teachers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Schedules",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Class",
                table: "Class",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attendance",
                table: "Attendance",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_ScheduleId",
                table: "Teachers",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ClassId",
                table: "Sessions",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_Day_SessionNumber",
                table: "Sessions",
                columns: new[] { "Day", "SessionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TeacherId",
                table: "Sessions",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_ClassId",
                table: "Schedules",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_Sessions_SessionId",
                table: "Attendance",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_Students_StudentId",
                table: "Attendance",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Class_ClassId",
                table: "Schedules",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id");

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
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Class_ClassId",
                table: "Students",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Schedules_ScheduleId",
                table: "Teachers",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");
        }
    }
}
