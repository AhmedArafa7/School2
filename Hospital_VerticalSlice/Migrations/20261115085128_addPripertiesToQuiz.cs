using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace School_VerticalSlice.Migrations
{
    /// <inheritdoc />
    public partial class addPripertiesToQuiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessedClasses",
                table: "Quizzes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "Duration",
                table: "Quizzes",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateTime>(
                name: "Endtime",
                table: "Quizzes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Starttime",
                table: "Quizzes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessedClasses",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Endtime",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Starttime",
                table: "Quizzes");
        }
    }
}
