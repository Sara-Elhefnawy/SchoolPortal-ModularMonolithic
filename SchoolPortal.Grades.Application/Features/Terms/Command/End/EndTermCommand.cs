using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Terms.Command.End;

public sealed record EndTermCommand(int TermId) : IRequest<Result>;