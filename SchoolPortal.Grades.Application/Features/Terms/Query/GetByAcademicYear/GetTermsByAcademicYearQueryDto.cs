namespace SchoolPortal.Grades.Application.Features.Terms.Query.GetByAcademicYear;

public sealed record GetTermsByAcademicYearQueryDto(
    int Id,
    string Name,
    string Type,
    DateOnly StartDate,
    DateOnly EndDate,
    int AcademicYearId,
    bool HasEnded);