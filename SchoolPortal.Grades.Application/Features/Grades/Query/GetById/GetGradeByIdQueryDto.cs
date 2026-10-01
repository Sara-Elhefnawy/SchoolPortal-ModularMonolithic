namespace SchoolPortal.Grades.Application.Features.Grades.Query.GetById;

public sealed record GetGradeByIdQueryDto(int Id, int StudentId, int AssessmentId, decimal Score, string Status, string StudentName = "", string AssessmentName = "");