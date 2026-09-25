using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Draft;

public sealed record DraftGradeCommand(int GradeId) : IRequest<Result>;