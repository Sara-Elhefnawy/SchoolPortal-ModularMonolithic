using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Students.Application.Features.Students.Commands.Graduate;

public sealed record GraduateStudentCommand(int StudentId, DateOnly GraduationDate) : IRequest<Result>;