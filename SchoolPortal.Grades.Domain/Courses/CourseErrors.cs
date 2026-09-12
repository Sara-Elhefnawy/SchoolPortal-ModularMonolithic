using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Grades.Domain.Courses;

public static class CourseErrors
{
    public static readonly Error InvalidName =
        Error.Validation("Course.InvalidName", "Name is required and must not exceed 100 characters.");

    public static readonly Error InvalidCode =
        Error.Validation("Course.InvalidCode", "Code is required and must not exceed 20 characters.");
    
    public static readonly Error InvalidCreditHours =
        Error.Validation("Course.InvalidCreditHours", "Credit hours must be greater than 0.");
}