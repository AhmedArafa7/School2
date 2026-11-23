using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace School_VerticalSlice.Migrations
{
    /// <inheritdoc />
    public partial class addMorePripertiesToQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizStudent");

            migrationBuilder.RenameColumn(
                name: "AccessedClasses",
                table: "Quizzes",
                newName: "Subject");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ClassQuiz",
                columns: table => new
                {
                    ClassesId = table.Column<int>(type: "int", nullable: false),
                    QuizzesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassQuiz", x => new { x.ClassesId, x.QuizzesId });
                    table.ForeignKey(
                        name: "FK_ClassQuiz_Classes_ClassesId",
                        column: x => x.ClassesId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassQuiz_Quizzes_QuizzesId",
                        column: x => x.QuizzesId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassQuiz_QuizzesId",
                table: "ClassQuiz",
                column: "QuizzesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassQuiz");

            migrationBuilder.RenameColumn(
                name: "Subject",
                table: "Quizzes",
                newName: "AccessedClasses");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QuizStudent",
                columns: table => new
                {
                    QuizzesId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizStudent", x => new { x.QuizzesId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_QuizStudent_Quizzes_QuizzesId",
                        column: x => x.QuizzesId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizStudent_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizStudent_StudentId",
                table: "QuizStudent",
                column: "StudentId");
        }
    }
}
