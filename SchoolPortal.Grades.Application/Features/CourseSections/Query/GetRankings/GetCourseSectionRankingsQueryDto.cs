namespace SchoolPortal.Grades.Application.Features.CourseSections.Query.GetRankings;

public sealed record GetCourseSectionRankingsQueryDto(
    int Rank,
    int StudentId,
    string StudentName,
    decimal WeightedAverage);