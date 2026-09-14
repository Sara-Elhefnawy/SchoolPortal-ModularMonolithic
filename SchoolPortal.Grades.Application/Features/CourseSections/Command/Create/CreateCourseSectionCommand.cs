using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.CourseSections.Command.Create;

public sealed record CreateCourseSectionCommand(int CourseId, int TermId) : IRequest<Result<int>>;