using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Directories;

namespace SchoolPortal.Students.Application.Features.Students.Query.GetAll;

public sealed class GetAllStudentsQueryHandler (IStudentDirectory directory)
    : IRequestHandler<GetAllStudentsQuery, Result<IReadOnlyList<GetAllStudentsQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetAllStudentsQueryDto>>> Handle(GetAllStudentsQuery request, CancellationToken ct = default)
    {
        var students = await directory.GetAllAsync(ct);

        return Result<IReadOnlyList<GetAllStudentsQueryDto>>.Success(students);
    }
}