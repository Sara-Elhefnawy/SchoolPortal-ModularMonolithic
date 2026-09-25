using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Attendances.Application.Features.Sessions.Query.GetByCourseSection;

public sealed record GetSessionsByCourseSectionQuery(int CourseSectionId) : IRequest<Result<IReadOnlyList<GetSessionsByCourseSectionQueryDto>>>;