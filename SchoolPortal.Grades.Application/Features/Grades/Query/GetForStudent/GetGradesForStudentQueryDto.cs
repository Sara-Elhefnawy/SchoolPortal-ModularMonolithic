namespace SchoolPortal.Grades.Application.Features.Grades.Query.GetForStudent;

public sealed record GetGradesForStudentQueryDto(
    int Id,
    int StudentId,
    int AssessmentId,
    string AssessmentName,
    decimal? Score,
    string Status,
    DateTime UpdatedAt);