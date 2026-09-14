using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Publish;

public sealed class PublishGradeCommandHandler(IGradeDirectory directory)
    : IRequestHandler<PublishGradeCommand, Result>
{
    public async Task<Result> Handle(PublishGradeCommand request, CancellationToken ct)
    {
        var grade = await directory.FindAsync(request.GradeId, ct);
        if (grade is null)
            return Result.Failure(GradeErrors.NotFound);

        var result = grade.Publish();
        if (result.IsFailure)
            return result;

        await directory.UpdateAsync(grade, ct);
        return Result.Success();
    }
}