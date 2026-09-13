using MediatR;
using Microsoft.EntityFrameworkCore;
using SchoolPortal.Attendances.Application.DbContexts;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Query.GetAttendanceForOccurrence;

public sealed class GetAttendanceForOccurrenceQueryHandler(IAttendancesDbContext db)
    : IRequestHandler<GetAttendanceForOccurrenceQuery, Result<List<GetAttendanceForOccurrenceQueryDto>>>
{
    public async Task<Result<List<GetAttendanceForOccurrenceQueryDto>>> Handle(
        GetAttendanceForOccurrenceQuery request, CancellationToken ct)
    {
        var rows = await db.Attendances
            .Where(a => a.SessionOccurrenceId == request.SessionOccurrenceId)
            .Select(a => new GetAttendanceForOccurrenceQueryDto(a.Id, a.StudentId, a.Status, a.Note))
            .ToListAsync(ct);

        return Result<List<GetAttendanceForOccurrenceQueryDto>>.Success(rows);
    }
}