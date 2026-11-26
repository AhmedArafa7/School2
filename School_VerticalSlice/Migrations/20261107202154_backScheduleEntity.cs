using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace School_VerticalSlice.Migrations
{
    /// <inheritdoc />
    public partial class backScheduleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Class_ClassId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Schedules_ScheduleId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Class",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsAttended",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Session",
                table: "Attendance");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Students",
                newName: "ClassId");

            migrationBuilder.RenameColumn(
                name: "day",
                table: "Attendance",
                newName: "SessionId");

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleId",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Schedules",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAttended",
                table: "Attendance",
                type: "bit",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Students_ClassId",
                table: "Students",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_ClassId",
                table: "Schedules",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_SessionId",
                table: "Attendance",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_Sessions_SessionId",
                table: "Attendance",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Sessions_Schedules_ScheduleId",
                table: "Sessions",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Class_ClassId",
                table: "Students",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_Sessions_SessionId",
                table: "Attendance");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Class_ClassId",
                table: "Schedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Class_ClassId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Schedules_ScheduleId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Class_ClassId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_ClassId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_ClassId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Attendance_SessionId",
                table: "Attendance");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "Students",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "Attendance",
                newName: "day");

            migrationBuilder.AddColumn<int>(
                name: "Class",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleId",
                table: "Sessions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsAttended",
                table: "Sessions",
                type: "bit",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IsAttended",
                table: "Attendance",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<int>(
                name: "Session",
                table: "Attendance",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Class_ClassId",
                table: "Sessions",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Schedules_ScheduleId",
                table: "Sessions",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");
        }
    }
}
