using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.AcademicYears;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.AcademicYears.Command.Create;

public sealed class CreateAcademicYearCommandHandler(IAcademicYearDirectory directory)
    : IRequestHandler<CreateAcademicYearCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateAcademicYearCommand request, CancellationToken ct)
    {
        var exists = await directory.ExistsWithNameAsync(request.Name.Trim(), ct);
        if (exists)
            return Result<int>.Failure(AcademicYearErrors.AlreadyExists);

        var yearResult = AcademicYear.Create(request.Name, request.StartDate, request.EndDate);
        if (yearResult.IsFailure)
            return Result<int>.Failure(yearResult.Error);

        await directory.AddAsync(yearResult.Value, ct);
        return Result<int>.Success(yearResult.Value.Id);
    }
}