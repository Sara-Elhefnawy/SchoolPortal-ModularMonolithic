using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Unlock;

public sealed record UnlockGradeCommand(int GradeId) : IRequest<Result>;