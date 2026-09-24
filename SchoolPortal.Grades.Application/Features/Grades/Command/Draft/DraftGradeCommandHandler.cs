using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Persistence;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Draft;

public sealed class DraftGradeCommandHandler(
    IGradeDirectory directory,
    IGradesUnitOfWork unitOfWork)
: IRequestHandler<DraftGradeCommand, Result>
{
public async Task<Result> Handle(DraftGradeCommand request, CancellationToken ct)
{
    var grade = await directory.GetByIdAsync(request.GradeId, ct);
    if (grade is null)
        return Result.Failure(GradeErrors.NotFound);

    var result = grade.Draft();
    if (result.IsFailure)
        return result;

    await unitOfWork.SaveChangesAsync(ct);
        
    return Result.Success();
}
}