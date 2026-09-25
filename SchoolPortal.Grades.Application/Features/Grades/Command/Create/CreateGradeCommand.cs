using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Create;

public sealed record CreateGradeCommand(int StudentId, int AssessmentId, decimal Score) : IRequest<Result<int>>;