using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace School_VerticalSlice.Migrations
{
    /// <inheritdoc />
    public partial class addStudentQuizEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Answer",
                table: "Questions",
                newName: "CorrectAnswer");

            migrationBuilder.CreateTable(
                name: "StudentQuiz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalDegree = table.Column<float>(type: "real", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentQuiz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentQuiz_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentQuiz_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuiz_QuizId",
                table: "StudentQuiz",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentQuiz_StudentId",
                table: "StudentQuiz",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentQuiz");

            migrationBuilder.RenameColumn(
                name: "CorrectAnswer",
                table: "Questions",
                newName: "Answer");
        }
    }
}
