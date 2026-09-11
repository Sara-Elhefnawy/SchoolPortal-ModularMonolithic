using MediatR;
using SchoolPortal.Attendance.Domain.Attendances;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendance.Application.Features.Command;

public record MarkAttendanceCommand(int StudentId, int SessionOccurrenceId, AttendanceStatus Status, string? Note) : IRequest<Result>;