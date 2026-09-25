using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Persistence;
using SchoolPortal.Grades.Domain.AcademicYears;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.AcademicYears.Command.Create;

public sealed class CreateAcademicYearCommandHandler(
    IAcademicYearDirectory directory,
    IGradesUnitOfWork unitOfWork)
    : IRequestHandler<CreateAcademicYearCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateAcademicYearCommand request, CancellationToken ct)
    {
        var exists = await directory.NameExistsAsync(request.Name.Trim(), ct);
        if (exists)
            return Result<int>.Failure(AcademicYearErrors.AlreadyExists);

        var yearResult = AcademicYear.Create(request.Name, request.StartDate, request.EndDate);
        if (yearResult.IsFailure)
            return Result<int>.Failure(yearResult.Error);

        directory.Add(yearResult.Value);
        await unitOfWork.SaveChangesAsync(ct);
        
        return Result<int>.Success(yearResult.Value.Id);
    }
}