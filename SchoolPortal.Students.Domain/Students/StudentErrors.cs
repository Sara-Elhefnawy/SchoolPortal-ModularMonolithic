using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Students.Domain.Students;

public static class StudentErrors
{
    public static readonly Error InvalidName =
        Error.Validation("Student.InvalidName", $"Name is required and must not exceed 100 characters.");

    public static readonly Error InvalidEmail = Error.Validation("Student.InvalidEmail", "Email address is invalid.");

    public static readonly Error InvalidPhoneNumber = Error.Validation("Student.InvalidPhoneNumber",
        "Phone number is required and must not exceed 20 characters.");

    public static readonly Error InvalidAddress = Error.Validation("Student.InvalidAddress",
        "Address is required and must not exceed 200 characters.");

    public static readonly Error InvalidDateOfBirth = Error.Validation("Student.InvalidDateOfBirth",
        "Date Of Birth is required and must be a valid date.");

    public static readonly Error InvalidEnrollmentDate = Error.Validation("Student.InvalidEnrollmentDate",
        "Enrollment Date is required and must be a valid date.");

    public static readonly Error InvalidGender = Error.Validation("Student.InvalidGender",
        "Gender is required and must be male of female.");
}