using SchoolPortal.SharedKernel.Domain;
using SchoolPortal.SharedKernel.Results;

namespace SchoolPortal.Grades.Domain.CourseSections;

// The bridge between a Course and the Term it's actually being taught in.
// Session, Enrollment, and Assessment all point here instead of at Course
// directly — that's what stops "which year/term is this?" from having three
// independent, uncoordinated answers scattered across three modules.
public sealed class CourseSection : BaseEntity
{
    public int CourseId { get; private set; }
    public int TermId { get; private set; }

    private CourseSection()
    {
    }

    private CourseSection(int courseId, int termId)
    {
        CourseId = courseId;
        TermId = termId;
    }

    public static Result<CourseSection> Create(int courseId, int termId)
    {
        var validationResult = Validate(courseId, termId);
        if (validationResult.IsFailure)
            return Result<CourseSection>.Failure(validationResult.Error);

        return Result<CourseSection>.Success(new CourseSection(courseId, termId));
    }

    private static Result Validate(int courseId, int termId)
    {
        if (courseId <= 0)
            return Result.Failure(CourseSectionErrors.InvalidCourseId);

        if (termId <= 0)
            return Result.Failure(CourseSectionErrors.InvalidTermId);

        return Result.Success();
    }
}