using System.Net.Mail;
using SchoolPortal.SharedKernel;
using SchoolPortal.SharedKernel.Results;
using SchoolPortal.Students.Domain.Addresses;
using SchoolPortal.Students.Domain.enums;

namespace SchoolPortal.Students.Domain.Students;

public sealed class Student : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public DateOnly DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public Address Address { get; private set; } = null!;
    public DateOnly EnrollmentDate { get; private set; }
    public StudentStatus Status { get; private set; }
    
    public Student() {}

    private Student(
        string name,
        string email,
        string phoneNumber,
        Address address,
        DateOnly dateOfBirth,
        Gender gender,
        DateOnly enrollmentDate)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        EnrollmentDate = enrollmentDate;
        Status = StudentStatus.Active;
    }

    public static Result<Student> Create(
        string name,
        string email,
        string phoneNumber,
        Address address,
        DateOnly dateOfBirth,
        Gender gender,
        DateOnly enrollmentDate)
    {
        var validationResult = ValidateAll(
            name,
            email,
            phoneNumber,
            address,
            dateOfBirth,
            gender,
            enrollmentDate);

        if (validationResult.IsFailure)
            return Result<Student>.Failure(validationResult.Error);

        var student = new Student(
            name.Trim(),
            email.Trim().ToLowerInvariant(),
            phoneNumber.Trim(),
            address,
            dateOfBirth,
            gender,
            enrollmentDate);

        return Result<Student>.Success(student);
    }

    private static Result ValidateAll(
        string name,
        string email,
        string phoneNumber,
        Address address,
        DateOnly dateOfBirth,
        Gender gender,
        DateOnly enrollmentDate)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
            return nameResult;

        var emailResult = ValidateEmail(email);
        if (emailResult.IsFailure)
            return emailResult;

        var phoneNumberResult = ValidatePhoneNumber(phoneNumber);
        if (phoneNumberResult.IsFailure)
            return phoneNumberResult;

        var addressResult = ValidateAddress(address);
        if (addressResult.IsFailure)
            return addressResult;

        var dateOfBirthResult = ValidateDateOfBirth(dateOfBirth);
        if (dateOfBirthResult.IsFailure)
            return dateOfBirthResult;
        
        var enrollmentDateResult = ValidateEnrollmentDate(enrollmentDate);
        if (enrollmentDateResult.IsFailure)
            return enrollmentDateResult;
        
        var genderResult = ValidateGender(gender);
        if (genderResult.IsFailure)
            return genderResult;

        return Result.Success();
    }

    private static Result ValidateEnrollmentDate(DateOnly enrollmentDate)
        => enrollmentDate > DateOnly.FromDateTime(DateTime.Now) ? Result.Failure(StudentErrors.InvalidEnrollmentDate) : Result.Success();

    private static Result ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            name.Trim().Length > Constants.MaxNameLength)
            return Result.Failure(StudentErrors.InvalidName);

        return Result.Success();
    }

    private static Result ValidateEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            email.Trim().Length > Constants.MaxEmailLength)
            return Result.Failure(StudentErrors.InvalidEmail);

        try
        {
            var normalizedEmail = email.Trim();
            var mailAddress = new MailAddress(normalizedEmail);

            if (!string.Equals(
                    mailAddress.Address,
                    normalizedEmail,
                    StringComparison.OrdinalIgnoreCase))
                return Result.Failure(StudentErrors.InvalidEmail);
        }
        catch (FormatException)
        {
            return Result.Failure(StudentErrors.InvalidEmail);
        }

        return Result.Success();
    }

    private static Result ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber) ||
            phoneNumber.Trim().Length > Constants.MaxPhoneNumberLength)
            return Result.Failure(StudentErrors.InvalidPhoneNumber);

        return Result.Success();
    }

    private static Result ValidateAddress(Address address)
    {
        if (address is null)
            return Result.Failure(StudentErrors.InvalidAddress);

        return Result.Success();
    }

    private static Result ValidateDateOfBirth(DateOnly dateOfBirth)
        => dateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow) ? Result.Failure(StudentErrors.InvalidDateOfBirth) : Result.Success();

    private static Result ValidateGender(Gender gender)
        => !Enum.IsDefined(gender) ? Result.Failure(StudentErrors.InvalidGender) : Result.Success();
}