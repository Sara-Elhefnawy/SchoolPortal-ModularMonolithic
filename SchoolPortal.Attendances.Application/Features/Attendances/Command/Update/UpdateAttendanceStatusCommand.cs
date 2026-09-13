using MediatR;
using SchoolPortal.Attendances.Domain.Attendances;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Command.Update;

public sealed record UpdateAttendanceStatusCommand(int AttendanceId, AttendanceStatus Status, string? Note) : IRequest<Result>;