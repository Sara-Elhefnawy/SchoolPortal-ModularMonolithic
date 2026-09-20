using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolPortal.Students.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AuthAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "students",
                table: "Students",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "students",
                table: "Students",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "students",
                table: "Enrollments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "students",
                table: "Enrollments",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "students",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "students",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "students",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "students",
                table: "Enrollments");
        }
    }
}
