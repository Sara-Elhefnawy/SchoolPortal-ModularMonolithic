using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Attendances.Application.Directories;
using SchoolPortal.SharedKernel.Lookups;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Query.GetAttendanceForOccurrence;

public sealed class GetAttendanceForOccurrenceQueryHandler(
    IAttendanceDirectory attendances,
    IStudentLookup studentNames)
    : IRequestHandler<GetAttendanceForOccurrenceQuery, Result<IReadOnlyList<GetAttendanceForOccurrenceQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetAttendanceForOccurrenceQueryDto>>> Handle(
        GetAttendanceForOccurrenceQuery request, CancellationToken ct)
    {
        var rows = await attendances.GetForSessionAsync(request.SessionOccurrenceId, ct);

        var names = await studentNames.GetNamesAsync(rows.Select(r => r.StudentId).Distinct().ToList(), ct);

        var dtos = rows.Select(a => new GetAttendanceForOccurrenceQueryDto(a.Id, a.StudentId,
            names.GetValueOrDefault(a.StudentId, $"Student #{a.StudentId}"),
            // WHY ToString(): the page compares against "Present", "Late", ... and this doesn't depend on your JSON enum settings.
            a.Status.ToString(), a.Note)).OrderBy(d => d.StudentName).ToList();

        return Result<IReadOnlyList<GetAttendanceForOccurrenceQueryDto>>.Success(dtos);
    }
}