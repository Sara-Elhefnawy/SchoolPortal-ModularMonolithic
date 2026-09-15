using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Students.Application.Features.Students.Commands.Suspend;

public sealed record SuspendStudentCommand(int StudentId, string Reason) : IRequest<Result>;