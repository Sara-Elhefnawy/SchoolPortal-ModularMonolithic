using SchoolPortal.SharedKernel.Results.Errors;

namespace SchoolPortal.Students.Domain.Students;

public static class StudentErrors
{
    public static readonly Error NotFound = Error.Validation("Student.NotFound", "Student not found.");

    public static readonly Error InvalidName =
        Error.Validation("Student.InvalidName", $"Name is required and must not exceed {Constants.MaxNameLength} characters.");

    public static readonly Error InvalidEmail = Error.Validation("Student.InvalidEmail", "Email address is invalid.");

    public static readonly Error InvalidPhoneNumber = Error.Validation("Student.InvalidPhoneNumber",
        $"Phone number is required and must not exceed {Constants.MaxPhoneNumberLength} characters.");

    public static readonly Error InvalidAddress = Error.Validation("Student.InvalidAddress",
        "Address is required.");

    public static readonly Error InvalidDateOfBirth = Error.Validation("Student.InvalidDateOfBirth",
        "Date Of Birth is required and must be a valid date.");

    public static readonly Error InvalidAdmissionDate = Error.Validation("Student.InvalidAdmissionDate",
        "Admission Date is required and must be a valid date.");

    public static readonly Error InvalidGender = Error.Validation("Student.InvalidGender",
        "Gender is required and must be male or female.");
    
    public static readonly Error EmailAlreadyExists = Error.Validation("Student.EmailAlreadyExists",
        "email already exists in database and can not be used.");
}