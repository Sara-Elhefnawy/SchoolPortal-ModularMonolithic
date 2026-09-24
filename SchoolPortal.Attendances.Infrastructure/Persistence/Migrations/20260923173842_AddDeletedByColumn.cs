using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolPortal.Attendances.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeletedByColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                schema: "attendance",
                table: "Sessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                schema: "attendance",
                table: "SessionOccurrences",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedByUserId",
                schema: "attendance",
                table: "Attendances",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "attendance",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "attendance",
                table: "SessionOccurrences");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                schema: "attendance",
                table: "Attendances");
        }
    }
}
