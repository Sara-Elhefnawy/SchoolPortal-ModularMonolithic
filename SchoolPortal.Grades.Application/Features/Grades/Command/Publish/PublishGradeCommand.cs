using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Publish;

public sealed record PublishGradeCommand(int GradeId) : IRequest<Result>;