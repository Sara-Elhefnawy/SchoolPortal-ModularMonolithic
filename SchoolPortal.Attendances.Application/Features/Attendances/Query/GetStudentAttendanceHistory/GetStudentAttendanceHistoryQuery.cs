using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Query.GetStudentAttendanceHistory;

public sealed record GetStudentAttendanceHistoryQuery(int StudentId) : IRequest<Result<IReadOnlyList<GetStudentAttendanceHistoryDto>>>;