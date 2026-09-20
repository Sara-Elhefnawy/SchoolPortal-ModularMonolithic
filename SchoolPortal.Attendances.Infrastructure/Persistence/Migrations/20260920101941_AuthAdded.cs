using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolPortal.Attendances.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AuthAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "attendance",
                table: "Sessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "attendance",
                table: "Sessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "attendance",
                table: "SessionOccurrences",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                schema: "attendance",
                table: "SessionOccurrences",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "attendance",
                table: "SessionOccurrences",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "attendance",
                table: "Attendances",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "attendance",
                table: "Attendances",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "attendance",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "attendance",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "attendance",
                table: "SessionOccurrences");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                schema: "attendance",
                table: "SessionOccurrences");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "attendance",
                table: "SessionOccurrences");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "attendance",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "attendance",
                table: "Attendances");
        }
    }
}
