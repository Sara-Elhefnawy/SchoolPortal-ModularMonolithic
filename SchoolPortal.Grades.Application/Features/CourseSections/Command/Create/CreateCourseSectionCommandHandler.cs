using MediatR;
using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Domain.Courses;
using SchoolPortal.Grades.Domain.CourseSections;
using SchoolPortal.Grades.Domain.Terms;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Application.Features.CourseSections.Command.Create;

public sealed class CreateCourseSectionCommandHandler(
    ICourseSectionDirectory sectionDirectory, ICourseDirectory courseDirectory, ITermDirectory termDirectory)
    : IRequestHandler<CreateCourseSectionCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateCourseSectionCommand request, CancellationToken ct)
    {
        var courseExists = await courseDirectory.FindAsync(request.CourseId, ct) is not null;
        if (!courseExists)
            return Result<int>.Failure(CourseErrors.NotFound);

        var termExists = await termDirectory.FindAsync(request.TermId, ct) is not null;
        if (!termExists)
            return Result<int>.Failure(TermErrors.NotFound);

        // Mirrors the unique index from CourseSectionConfiguration — same
        // "one section per course per term" assumption flagged back then.
        var duplicate = await sectionDirectory.ExistsAsync(request.CourseId, request.TermId, ct);
        if (duplicate)
            return Result<int>.Failure(CourseSectionErrors.AlreadyExists);

        var sectionResult = CourseSection.Create(request.CourseId, request.TermId);
        if (sectionResult.IsFailure)
            return Result<int>.Failure(sectionResult.Error);

        await sectionDirectory.AddAsync(sectionResult.Value, ct);
        return Result<int>.Success(sectionResult.Value.Id);
    }
}