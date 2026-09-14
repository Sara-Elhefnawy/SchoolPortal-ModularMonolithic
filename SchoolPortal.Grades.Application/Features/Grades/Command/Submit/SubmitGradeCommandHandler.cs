using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Submit;

public sealed class SubmitGradeCommandHandler(IGradeDirectory directory)
    : IRequestHandler<SubmitGradeCommand, Result>
{
    public async Task<Result> Handle(SubmitGradeCommand request, CancellationToken ct)
    {
        var grade = await directory.FindAsync(request.GradeId, ct);
        if (grade is null)
            return Result.Failure(GradeErrors.NotFound);

        var result = grade.Submit();
        if (result.IsFailure)
            return result;

        await directory.UpdateAsync(grade, ct);
        return Result.Success();
    }
}