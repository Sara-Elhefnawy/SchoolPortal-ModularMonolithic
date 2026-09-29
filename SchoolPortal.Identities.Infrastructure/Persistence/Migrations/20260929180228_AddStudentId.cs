using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolPortal.Identities.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                schema: "identity",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StudentId",
                schema: "identity",
                table: "AspNetUsers",
                column: "StudentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_StudentId",
                schema: "identity",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StudentId",
                schema: "identity",
                table: "AspNetUsers");
        }
    }
}
