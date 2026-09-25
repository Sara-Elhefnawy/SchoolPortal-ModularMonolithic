using SchoolPortal.SharedKernel.Mediator;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.AcademicYears.Query.GetAll;

public sealed class GetAllAcademicYearsQueryHandler(IAcademicYearDirectory directory)
    : IRequestHandler<GetAllAcademicYearsQuery, Result<IReadOnlyList<GetAllAcademicYearsQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetAllAcademicYearsQueryDto>>> Handle(
        GetAllAcademicYearsQuery request,
        CancellationToken ct)
    {
        var years = await directory.GetAllAsync(ct);

        return Result<IReadOnlyList<GetAllAcademicYearsQueryDto>>.Success(years);
    }
}