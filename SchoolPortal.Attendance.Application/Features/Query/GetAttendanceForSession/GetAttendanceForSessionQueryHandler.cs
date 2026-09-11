using MediatR;
using SchoolPortal.Attendance.Application.Abstractions;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendance.Application.Features.Query.GetAttendanceForSession;

public sealed class GetAttendanceForSessionQueryHandler(IAttendanceDirectory directory)
    : IRequestHandler<GetAttendanceForSessionQuery, Result<IReadOnlyList<GetAttendanceForSessionDto>>>
{
    public async Task<Result<IReadOnlyList<GetAttendanceForSessionDto>>> Handle(GetAttendanceForSessionQuery query,
        CancellationToken ct)
    {
        var records = await directory.GetForSessionAsync(query.SessionOccurrenceId, ct);

        var dtos = records.Select(r => new GetAttendanceForSessionDto(r.StudentId, r.Status, r.Note)).ToList();

        return Result<IReadOnlyList<GetAttendanceForSessionDto>>.Success(dtos);
    }
}