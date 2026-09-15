using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Persistence;
using SchoolPortal.Grades.Domain.AcademicYears;
using SchoolPortal.Grades.Domain.Terms;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Terms.Command.Create;

public sealed class CreateTermCommandHandler(
    ITermDirectory termDirectory, 
    IAcademicYearDirectory yearDirectory,
    IGradesUnitOfWork unitOfWork)
    : IRequestHandler<CreateTermCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateTermCommand request, CancellationToken ct)
    {
        // Cross-aggregate check, both within Grades — no module boundary crossed,
        // so a direct directory call here (unlike Students/CourseSectionId
        // elsewhere) is completely fine.
        var yearExists = await yearDirectory.GetByIdAsync(request.AcademicYearId, ct) is not null;
        if (!yearExists)
            return Result<int>.Failure(AcademicYearErrors.NotFound);

        var duplicate = await termDirectory.AcademicYearAndNameExistsAsync(request.AcademicYearId, request.Name.Trim(), ct);
        if (duplicate)
            return Result<int>.Failure(TermErrors.AlreadyExists);

        var termResult = Term.Create(request.Name, request.Type, request.StartDate, request.EndDate, request.AcademicYearId);
        if (termResult.IsFailure)
            return Result<int>.Failure(termResult.Error);

        termDirectory.Add(termResult.Value);
        await unitOfWork.SaveChangesAsync(ct);
        
        return Result<int>.Success(termResult.Value.Id);
    }
}