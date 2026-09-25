using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Attendances.Domain.Attendances;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Command.Record;

public record RecordAttendanceCommand(int StudentId, int SessionOccurrenceId, AttendanceStatus Status, string? Note) : IRequest<Result<int>>;