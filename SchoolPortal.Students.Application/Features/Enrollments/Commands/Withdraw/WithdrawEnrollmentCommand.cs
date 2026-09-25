using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Students.Application.Features.Enrollments.Commands.Withdraw;

public sealed record WithdrawEnrollmentCommand(int EnrollmentId) : IRequest<Result>;