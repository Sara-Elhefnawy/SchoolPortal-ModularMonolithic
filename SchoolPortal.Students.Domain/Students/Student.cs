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
    // the date this student was admitted to the school
    public DateOnly AdmissionDate { get; private set; }
    public StudentStatus Status { get; private set; }
    
    private Student() {}

    private Student(
        string name,
        string email,
        string phoneNumber,
        Address address,
        DateOnly dateOfBirth,
        Gender gender,
        DateOnly admissionDate)
    {
        Name = name;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        DateOfBirth = dateOfBirth;
        Gender = gender;
        AdmissionDate = admissionDate;
        Status = StudentStatus.Active;
    }

    public static Result<Student> Create(
        string name,
        string email,
        string phoneNumber,
        Address address,
        DateOnly dateOfBirth,
        Gender gender,
        DateOnly admissionDate)
    {
        var validationResult = ValidateAll(
            name,
            email,
            phoneNumber,
            address,
            dateOfBirth,
            gender,
            admissionDate);

        if (validationResult.IsFailure)
            return Result<Student>.Failure(validationResult.Error);

        var student = new Student(
            name.Trim(),
            email.Trim().ToLowerInvariant(),
            phoneNumber.Trim(),
            address,
            dateOfBirth,
            gender,
            admissionDate);

        return Result<Student>.Success(student);
    }

    private static Result ValidateAll(
        string name,
        string email,
        string phoneNumber,
        Address address,
        DateOnly dateOfBirth,
        Gender gender,
        DateOnly admissionDate)
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
        
        var admissionDateResult = ValidateAdmissionDate(admissionDate);
        if (admissionDateResult.IsFailure)
            return admissionDateResult;
        
        var genderResult = ValidateGender(gender);
        if (genderResult.IsFailure)
            return genderResult;

        return Result.Success();
    }

    private static Result ValidateAdmissionDate(DateOnly admissionDate)
        => admissionDate > DateOnly.FromDateTime(DateTime.Now) ? Result.Failure(StudentErrors.InvalidAdmissionDate) : Result.Success();

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