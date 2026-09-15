using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Persistence;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Lock;

public sealed class LockGradeCommandHandler(
    IGradeDirectory directory,
    IGradesUnitOfWork unitOfWork)
    : IRequestHandler<LockGradeCommand, Result>
{
    public async Task<Result> Handle(LockGradeCommand request, CancellationToken ct)
    {
        var grade = await directory.GetByIdAsync(request.GradeId, ct);
        if (grade is null)
            return Result.Failure(GradeErrors.NotFound);

        var result = grade.Lock();
        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(ct);
        
        return Result.Success();
    }
}