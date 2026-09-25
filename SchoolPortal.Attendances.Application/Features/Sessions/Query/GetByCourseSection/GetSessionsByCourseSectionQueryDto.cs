namespace SchoolPortal.Attendances.Application.Features.Sessions.Query.GetByCourseSection;

public sealed record GetSessionsByCourseSectionQueryDto(
    int Id,
    int CourseSectionId,
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime
    );