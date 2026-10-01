using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Directories;

namespace SchoolPortal.Students.Application.Features.Students.Query.GetNamesByIds;

public sealed class GetStudentNamesByIdsQueryHandler(IStudentDirectory directory)
    : IRequestHandler<GetStudentNamesByIdsQuery, Result<IReadOnlyDictionary<int, string>>>
{
    public async Task<Result<IReadOnlyDictionary<int, string>>> Handle(GetStudentNamesByIdsQuery request,
        CancellationToken ct = default)
    {
        var names = await directory.GetNamesAsync(request.Ids, ct);
        return Result<IReadOnlyDictionary<int, string>>.Success(names);
    }
}