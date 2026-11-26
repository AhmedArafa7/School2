using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace School_VerticalSlice.Migrations
{
    /// <inheritdoc />
    public partial class removeScheduleEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Session_Class_ClassId",
                table: "Session");

            migrationBuilder.DropForeignKey(
                name: "FK_Session_Schedules_ScheduleId",
                table: "Session");

            migrationBuilder.DropTable(
                name: "ScheduleTeacher");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Session",
                table: "Session");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Session");

            migrationBuilder.RenameTable(
                name: "Session",
                newName: "Sessions");

            migrationBuilder.RenameIndex(
                name: "IX_Session_ScheduleId",
                table: "Sessions",
                newName: "IX_Sessions_ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Session_ClassId",
                table: "Sessions",
                newName: "IX_Sessions_ClassId");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Teachers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClassCode",
                table: "Class",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<int>(
                name: "SessionNumber",
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

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_ScheduleId",
                table: "Teachers",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TeacherId",
                table: "Sessions",
                column: "TeacherId");

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
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Teachers_TeacherId",
                table: "Sessions",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Schedules_ScheduleId",
                table: "Teachers",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Class_ClassId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Schedules_ScheduleId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Teachers_TeacherId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Schedules_ScheduleId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_ScheduleId",
                table: "Teachers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sessions",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_TeacherId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "ClassCode",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "IsAttended",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "SessionNumber",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Sessions");

            migrationBuilder.RenameTable(
                name: "Sessions",
                newName: "Session");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_ScheduleId",
                table: "Session",
                newName: "IX_Session_ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Sessions_ClassId",
                table: "Session",
                newName: "IX_Session_ClassId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Class",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "ScheduleId",
                table: "Session",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Session",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Session",
                table: "Session",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Session_Class_ClassId",
                table: "Session",
                column: "ClassId",
                principalTable: "Class",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Session_Schedules_ScheduleId",
                table: "Session",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
