using MediatR;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.CourseSections.Query.GetByTerm;

public sealed record GetCourseSectionsByTermQuery(int TermId) : IRequest<Result<IReadOnlyList<GetCourseSectionsByTermQueryDto>>>;