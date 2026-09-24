namespace SchoolPortal.Grades.Application.Features.Assessments.Query.GetByCourseSection;

public sealed record GetAssessmentsByCourseSectionQueryDto(
    int Id,
    int CourseSectionId,
    string Name,
    string Type,
    decimal MaximumScore,
    decimal Weight,
    DateOnly Date);