using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Application.Directories;
using SchoolPortal.Students.Domain.Students;

namespace SchoolPortal.Students.Application.Features.Students.Query.GetById;

public sealed class GetStudentByIdQueryHandler (IStudentDirectory directory)
    : IRequestHandler<GetStudentByIdQuery, Result<Student>>
{
    public async Task<Result<Student>> Handle(GetStudentByIdQuery request, CancellationToken ct = default)
    {
        var student = await directory.GetByIdAsync(request.Id, ct);
        if (student == null)
            return Result<Student>.Failure(StudentErrors.NotFound);

        return Result<Student>.Success(student);
    }
}