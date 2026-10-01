using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Sessions.Query.GetToday;

public sealed class GetTodaysSessionsQueryHandler(ISessionDirectory sessions, TimeProvider clock)
    : IRequestHandler<GetTodaysSessionsQuery, Result<IReadOnlyList<GetTodaysSessionsQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetTodaysSessionsQueryDto>>> Handle(GetTodaysSessionsQuery request,
        CancellationToken ct)
    {
        // Local date, not UTC: Egypt is UTC+2/+3, so between midnight and about 3am
        // local time the UTC date is still "yesterday" and you'd get the wrong classes.
        var today = DateOnly.FromDateTime(clock.GetLocalNow().DateTime);

        var items = await sessions.GetForDateAsync(today, ct);

        return Result<IReadOnlyList<GetTodaysSessionsQueryDto>>.Success(items);
    }
}