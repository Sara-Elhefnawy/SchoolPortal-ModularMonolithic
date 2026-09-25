namespace SchoolPortal.Attendances.Application.Features.SessionOccurrences.Query.GetBySession;

public sealed record GetOccurrencesBySessionQueryDto(int Id, int SessionId, DateOnly Date);