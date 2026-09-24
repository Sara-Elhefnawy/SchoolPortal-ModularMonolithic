using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolPortal.Students.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedByColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                schema: "students",
                table: "Students",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                schema: "students",
                table: "Enrollments",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "students",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "students",
                table: "Enrollments");
        }
    }
}
