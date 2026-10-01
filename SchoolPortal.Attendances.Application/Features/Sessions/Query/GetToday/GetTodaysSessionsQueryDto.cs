namespace SchoolPortal.Attendances.Application.Features.Sessions.Query.GetToday;

public sealed record GetTodaysSessionsQueryDto(
    int SessionId,
    int CourseSectionId,
    TimeOnly StartTime,
    TimeOnly EndTime,
    int? OccurrenceId);