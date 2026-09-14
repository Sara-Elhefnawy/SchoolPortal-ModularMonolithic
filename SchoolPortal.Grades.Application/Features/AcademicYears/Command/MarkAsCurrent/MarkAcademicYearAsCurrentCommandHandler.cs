using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.AcademicYears;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.AcademicYears.Command.MarkAsCurrent;

public sealed class MarkAcademicYearAsCurrentCommandHandler(IAcademicYearDirectory directory)
    : IRequestHandler<MarkAcademicYearAsCurrentCommand, Result>
{
    public async Task<Result> Handle(MarkAcademicYearAsCurrentCommand request, CancellationToken ct)
    {
        var target = await directory.FindAsync(request.AcademicYearId, ct);
        if (target is null)
            return Result.Failure(AcademicYearErrors.NotFound);

        if (target.IsCurrent)
            return Result.Success(); // already current so not an error

        var currentYear = await directory.FindCurrentAsync(ct);
        if (currentYear is not null)
        {
            var unmarkResult = currentYear.UnmarkAsCurrent();
            if (unmarkResult.IsFailure)
                return unmarkResult;

            await directory.UpdateAsync(currentYear, ct);
        }

        var markResult = target.MarkAsCurrent();
        if (markResult.IsFailure)
            return markResult;

        await directory.UpdateAsync(target, ct);
        return Result.Success();
    }
}