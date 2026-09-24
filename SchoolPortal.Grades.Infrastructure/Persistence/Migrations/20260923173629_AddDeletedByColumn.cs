using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolPortal.Grades.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedByColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                schema: "grades",
                table: "Terms",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                schema: "grades",
                table: "Grades",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                schema: "grades",
                table: "CourseSections",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                schema: "grades",
                table: "Courses",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                schema: "grades",
                table: "Assessments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                schema: "grades",
                table: "AcademicYears",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "grades",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "grades",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "grades",
                table: "CourseSections");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "grades",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "grades",
                table: "Assessments");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "grades",
                table: "AcademicYears");
        }
    }
}
