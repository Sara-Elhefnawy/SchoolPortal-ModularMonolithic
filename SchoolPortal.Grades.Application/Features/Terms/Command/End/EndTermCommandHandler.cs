using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Persistence;
using SchoolPortal.Grades.Domain.Terms;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Terms.Command.End;

public sealed class EndTermCommandHandler(
    ITermDirectory directory,
    IGradesUnitOfWork unitOfWork)
    : IRequestHandler<EndTermCommand, Result>
{
    public async Task<Result> Handle(EndTermCommand request, CancellationToken ct)
    {
        var term = await directory.GetByIdAsync(request.TermId, ct);
        if (term is null)
            return Result.Failure(TermErrors.NotFound);

        var result = term.End();
        if (result.IsFailure)
            return result;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}