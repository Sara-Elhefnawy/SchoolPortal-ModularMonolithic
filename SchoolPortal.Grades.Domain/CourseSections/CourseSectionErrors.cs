using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Grades.Domain.CourseSections;

public static class CourseSectionErrors
{
    public static readonly Error NotFound = Error.Validation("CourseSection.NotFound", "Course section not found.");

    public static readonly Error InvalidCourseId =
        Error.Validation("CourseSection.InvalidCourseId", "Course id must be a positive number.");

    public static readonly Error InvalidTermId =
        Error.Validation("CourseSection.InvalidTermId", "Term id must be a positive number.");

    public static readonly Error AlreadyExists = Error.Validation("CourseSection.AlreadyExists", "Course section already exists.");
}