using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Unpublish;

public sealed record UnpublishGradeCommand(int GradeId) : IRequest<Result>;