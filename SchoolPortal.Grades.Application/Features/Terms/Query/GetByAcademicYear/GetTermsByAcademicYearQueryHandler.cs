using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.Terms.Query.GetByAcademicYear;

public sealed class GetTermsByAcademicYearQueryHandler(ITermDirectory directory)
    : IRequestHandler<GetTermsByAcademicYearQuery, Result<IReadOnlyList<GetTermsByAcademicYearQueryDto>>>
{
    public async Task<Result<IReadOnlyList<GetTermsByAcademicYearQueryDto>>> Handle(
        GetTermsByAcademicYearQuery request,
        CancellationToken ct)
    {
        var terms = await directory.GetByAcademicYearAsync(request.AcademicYearId, ct);

        return Result<IReadOnlyList<GetTermsByAcademicYearQueryDto>>.Success(terms);
    }
}