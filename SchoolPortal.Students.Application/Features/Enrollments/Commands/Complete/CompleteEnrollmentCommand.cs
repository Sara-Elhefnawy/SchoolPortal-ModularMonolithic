using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Students.Application.Features.Enrollments.Commands.Complete;

public sealed record CompleteEnrollmentCommand(int EnrollmentId) : IRequest<Result>;