using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Dtos;

namespace SchoolPortal.Students.Application.Features.Students.Commands.UpdatePhone;

public record UpdateStudentPhoneCommand(int StudentId, string Phone) : IRequest<Result<StudentSummaryDto>>;