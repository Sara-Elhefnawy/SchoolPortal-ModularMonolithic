using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Lock;

public sealed record LockGradeCommand(int GradeId) : IRequest<Result>;