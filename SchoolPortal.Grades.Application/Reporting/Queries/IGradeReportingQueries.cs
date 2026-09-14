using SchoolPortal.Grades.Application.Reporting.Dto;

namespace SchoolPortal.Grades.Application.Reporting.Queries;

public interface IGradeReportingQueries
{
    Task<IReadOnlyList<StudentRankingDto>> GetClassRankingsAsync(int courseSectionId, CancellationToken ct);
}