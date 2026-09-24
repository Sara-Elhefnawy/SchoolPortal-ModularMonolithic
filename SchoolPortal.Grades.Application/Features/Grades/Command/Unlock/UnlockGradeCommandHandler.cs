using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Persistence;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Unlock;

public sealed class UnlockGradeCommandHandler(
    IGradeDirectory directory,
    IGradesUnitOfWork unitOfWork)
    : IRequestHandler<UnlockGradeCommand, Result>
{
    public async Task<Result> Handle(UnlockGradeCommand request, CancellationToken ct)
    {
        var grade = await directory.GetByIdAsync(request.GradeId, ct);
        if (grade is null)
            return Result.Failure(GradeErrors.NotFound);

        var result = grade.OverrideUnlock();
        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(ct);
        
        return Result.Success();
    }
}