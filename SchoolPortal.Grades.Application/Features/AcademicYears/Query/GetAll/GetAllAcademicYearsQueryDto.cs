namespace SchoolPortal.Grades.Application.Features.AcademicYears.Query.GetAll;

public sealed record GetAllAcademicYearsQueryDto(
    int Id,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    bool IsCurrent);