using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Persistence;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Unpublish;

public sealed class UnpublishGradeCommandHandler(
    IGradeDirectory directory,
    IGradesUnitOfWork unitOfWork)
    : IRequestHandler<UnpublishGradeCommand, Result>
{
    public async Task<Result> Handle(UnpublishGradeCommand request, CancellationToken ct)
    {
        var grade = await directory.GetByIdAsync(request.GradeId, ct);
        if (grade is null)
            return Result.Failure(GradeErrors.NotFound);

        var result = grade.Unpublish();
        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}