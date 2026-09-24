namespace SchoolPortal.Grades.Application.Features.CourseSections.Query.GetByTerm;

public sealed record GetCourseSectionsByTermQueryDto(
    int Id,
    int CourseId,
    string Name,
    int TermId);