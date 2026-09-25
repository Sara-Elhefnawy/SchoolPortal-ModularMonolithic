using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.ReviseScore;

public sealed record ReviseGradeScoreCommand(int GradeId, decimal NewScore) : IRequest<Result>;