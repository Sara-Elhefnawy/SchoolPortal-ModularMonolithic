using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Features.Students.Commands.Dtos;

namespace SchoolPortal.Students.Application.Features.Students.Commands.UpdateEmail;

public sealed record UpdateStudentEmailCommand(int StudentId, string Email) :  IRequest<Result<StudentSummaryDto>>;