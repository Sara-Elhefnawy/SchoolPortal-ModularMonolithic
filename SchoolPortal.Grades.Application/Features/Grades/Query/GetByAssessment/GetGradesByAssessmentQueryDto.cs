namespace SchoolPortal.Grades.Application.Features.Grades.Query.GetByAssessment;

public sealed record GetGradesByAssessmentQueryDto(
    int Id,
    int StudentId,
    string StudentName,
    int AssessmentId,
    string AssessmentName,
    decimal? Score,
    string Status);