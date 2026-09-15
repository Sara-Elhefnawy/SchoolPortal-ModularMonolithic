using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Persistence;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Submit;

public sealed class SubmitGradeCommandHandler(
    IGradeDirectory directory,
    IGradesUnitOfWork unitOfWork)
    : IRequestHandler<SubmitGradeCommand, Result>
{
    public async Task<Result> Handle(SubmitGradeCommand request, CancellationToken ct)
    {
        var grade = await directory.GetByIdAsync(request.GradeId, ct);
        if (grade is null)
            return Result.Failure(GradeErrors.NotFound);

        var result = grade.Submit();
        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(ct);
        
        return Result.Success();
    }
}