using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Persistence;
using SchoolPortal.Grades.Domain.Courses;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Courses.Command.Create;

sealed class CreateCourseCommandHandler(
    ICourseDirectory directory,
    IGradesUnitOfWork unitOfWork)
    : IRequestHandler<CreateCourseCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateCourseCommand request, CancellationToken ct)
    {
        // Course.Create uppercases the code, so the duplicate check has to
        // compare against that same normalized form — checking the raw
        // request.Code here would miss "cs201" colliding with an existing "CS201".
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var duplicate = await directory.ExistsWithCodeAsync(normalizedCode, ct);
        if (duplicate)
            return Result<int>.Failure(CourseErrors.DuplicateCode);

        var courseResult = Course.Create(request.Name, request.Code, request.CreditHours);
        if (courseResult.IsFailure)
            return Result<int>.Failure(courseResult.Error);

        directory.Add(courseResult.Value);
        await unitOfWork.SaveChangesAsync(ct);
        
        return Result<int>.Success(courseResult.Value.Id);
    }
}