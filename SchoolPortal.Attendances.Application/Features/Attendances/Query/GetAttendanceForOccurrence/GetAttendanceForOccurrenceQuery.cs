using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Query.GetAttendanceForOccurrence;

public sealed record GetAttendanceForOccurrenceQuery(int SessionOccurrenceId) : IRequest<Result<IReadOnlyList<GetAttendanceForOccurrenceQueryDto>>>;