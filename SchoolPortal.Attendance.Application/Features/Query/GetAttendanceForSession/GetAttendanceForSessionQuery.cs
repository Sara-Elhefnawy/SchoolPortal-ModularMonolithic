using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendance.Application.Features.Query.GetAttendanceForSession;

public record GetAttendanceForSessionQuery(int SessionOccurrenceId) : IRequest<Result<IReadOnlyList<GetAttendanceForSessionDto>>>;