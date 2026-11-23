using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace School_VerticalSlice.Migrations
{
    /// <inheritdoc />
    public partial class FixClassStudentsRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Classes_ClassId1",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_ClassId1",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ClassId1",
                table: "Students");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassId1",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_ClassId1",
                table: "Students",
                column: "ClassId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Classes_ClassId1",
                table: "Students",
                column: "ClassId1",
                principalTable: "Classes",
                principalColumn: "Id");
        }
    }
}
