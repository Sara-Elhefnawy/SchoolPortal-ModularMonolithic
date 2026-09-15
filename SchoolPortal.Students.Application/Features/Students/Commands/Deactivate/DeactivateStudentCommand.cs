using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Students.Application.Features.Students.Commands.Deactivate;

public sealed record DeactivateStudentCommand(int StudentId) : IRequest<Result>;