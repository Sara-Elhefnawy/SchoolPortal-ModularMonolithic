using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Query.GetAttendanceForOccurrence;

public sealed class GetAttendanceForOccurrenceQueryHandler(IAttendanceDirectory directory)
    : IRequestHandler<GetAttendanceForOccurrenceQuery, Result<IReadOnlyList<GetAttendanceForOccurrenceQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetAttendanceForOccurrenceQueryDto>>> Handle(
        GetAttendanceForOccurrenceQuery request, CancellationToken ct)
    {
        // GetForSessionAsync already existed on your interface — this used to bypass it
        // and query an exposed IQueryable directly instead.
        var attendances = await directory.GetForSessionAsync(request.SessionOccurrenceId, ct);

        var rows = attendances
            .Select(a => new GetAttendanceForOccurrenceQueryDto(a.Id, a.StudentId, a.Status, a.Note))
            .ToList();

        return Result<IReadOnlyList<GetAttendanceForOccurrenceQueryDto>>.Success(rows);
    }
}