using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Students.Application.Features.Students.Query.GetNamesByIds;

public sealed record GetStudentNamesByIdsQuery(IReadOnlyCollection<int> Ids)
    : IRequest<Result<IReadOnlyDictionary<int, string>>>;