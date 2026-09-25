using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Submit;

public sealed record SubmitGradeCommand(int GradeId) : IRequest<Result>;