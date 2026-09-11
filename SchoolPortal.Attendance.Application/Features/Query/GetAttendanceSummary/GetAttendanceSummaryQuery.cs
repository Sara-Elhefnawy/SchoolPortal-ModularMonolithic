using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendance.Application.Features.Query.GetAttendanceSummary;

public record GetAttendanceSummaryQuery(int StudentId) : IRequest<Result<GetAttendanceSummaryDto>>;