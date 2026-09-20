using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolPortal.Grades.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AuthAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Terms_AcademicYearId",
                schema: "grades",
                table: "Terms");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "grades",
                table: "Terms",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasEnded",
                schema: "grades",
                table: "Terms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "grades",
                table: "Terms",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "grades",
                table: "Grades",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "grades",
                table: "Grades",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "grades",
                table: "CourseSections",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "grades",
                table: "CourseSections",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CreditHours",
                schema: "grades",
                table: "Courses",
                type: "numeric(4,1)",
                precision: 4,
                scale: 1,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(5,2)");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "grades",
                table: "Courses",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "grades",
                table: "Courses",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "grades",
                table: "Assessments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "grades",
                table: "Assessments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                schema: "grades",
                table: "AcademicYears",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                schema: "grades",
                table: "AcademicYears",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Terms_AcademicYearId_Name",
                schema: "grades",
                table: "Terms",
                columns: new[] { "AcademicYearId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicYears_IsCurrent",
                schema: "grades",
                table: "AcademicYears",
                column: "IsCurrent",
                unique: true,
                filter: "\"IsCurrent\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicYears_Name",
                schema: "grades",
                table: "AcademicYears",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Terms_AcademicYearId_Name",
                schema: "grades",
                table: "Terms");

            migrationBuilder.DropIndex(
                name: "IX_AcademicYears_IsCurrent",
                schema: "grades",
                table: "AcademicYears");

            migrationBuilder.DropIndex(
                name: "IX_AcademicYears_Name",
                schema: "grades",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "grades",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "HasEnded",
                schema: "grades",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "grades",
                table: "Terms");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "grades",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "grades",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "grades",
                table: "CourseSections");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "grades",
                table: "CourseSections");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "grades",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "grades",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "grades",
                table: "Assessments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "grades",
                table: "Assessments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                schema: "grades",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                schema: "grades",
                table: "AcademicYears");

            migrationBuilder.AlterColumn<decimal>(
                name: "CreditHours",
                schema: "grades",
                table: "Courses",
                type: "numeric(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(4,1)",
                oldPrecision: 4,
                oldScale: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Terms_AcademicYearId",
                schema: "grades",
                table: "Terms",
                column: "AcademicYearId");
        }
    }
}
