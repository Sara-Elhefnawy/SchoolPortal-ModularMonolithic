namespace SchoolPortal.Grades.Application.Reporting.Dto;

public sealed class StudentRankingDto
{
    public int StudentId { get; init; }

    public decimal WeightedAverage { get; init; }

    public long Rank { get; init; }
}