using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Students.Query.GetAll;

public sealed class GetAllStudentsQueryHandler (IStudentDirectory directory)
    : IRequestHandler<GetAllStudentsQuery, Result<IReadOnlyList<GetAllStudentsQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetAllStudentsQueryDto>>> Handle(GetAllStudentsQuery request, CancellationToken ct = default)
    {
        // The dropdown sends text. Convert it to the enum here so the directory filters on a real enum column.
        StudentStatus? status = null;
        if (Enum.TryParse<StudentStatus>(request.Status, ignoreCase: true, out var parsed) && Enum.IsDefined(parsed))
            status = parsed;
        
        var students = await directory.GetAllAsync(request.Search, status, ct);

        return Result<IReadOnlyList<GetAllStudentsQueryDto>>.Success(students);
    }
}