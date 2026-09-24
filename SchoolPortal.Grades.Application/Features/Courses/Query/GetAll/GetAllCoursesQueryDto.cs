namespace SchoolPortal.Grades.Application.Features.Courses.Query.GetAll;

public sealed record GetAllCoursesQueryDto(
    int Id,
    string Name,
    string Code,
    decimal CreditHours);