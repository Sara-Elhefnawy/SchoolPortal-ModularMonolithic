using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.CourseSections.Query.GetRankings;

public sealed record GetCourseSectionRankingsQuery(int CourseSectionId)
    : IRequest<Result<IReadOnlyList<GetCourseSectionRankingsQueryDto>>>;