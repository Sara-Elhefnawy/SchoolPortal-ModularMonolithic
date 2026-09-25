using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Assessments.Query.GetByCourseSection;

public sealed record GetAssessmentsBySectionQuery(int CourseSectionId)
    : IRequest<Result<IReadOnlyList<GetAssessmentsByCourseSectionQueryDto>>>;