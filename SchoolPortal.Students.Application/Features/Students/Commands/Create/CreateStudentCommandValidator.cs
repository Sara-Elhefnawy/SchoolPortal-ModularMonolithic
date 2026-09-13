using FluentValidation;

namespace SchoolPortal.Students.Application.Features.Students.Commands.Create;

public sealed class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
{
    public CreateStudentCommandValidator()
    {
        RuleFor(s => s.Name).NotEmpty().WithMessage("Name is required.").WithErrorCode("Student.Name.Empty");

        RuleFor(s => s.Email).NotEmpty().EmailAddress().WithMessage("A valid email address is required.")
            .WithErrorCode("Student.Email.Invalid");

        RuleFor(s => s.PhoneNumber).NotEmpty().WithMessage("Phone number is required.")
            .WithErrorCode("Student.PhoneNumber.Empty");

        RuleFor(s => s.DateOfBirth).NotEqual(default(DateOnly)).WithMessage("Date of birth is required.")
            .WithErrorCode("Student.DateOfBirth.Empty");

        RuleFor(s => s.Gender).IsInEnum().WithMessage("Gender must be a valid value.")
            .WithErrorCode("Student.Gender.Invalid");

        RuleFor(s => s.AdmissionDate).NotEqual(default(DateOnly)).WithMessage("Admission date is required.")
            .WithErrorCode("Student.AdmissionDate.Empty");

        RuleFor(s => s.City).NotEmpty().WithMessage("City is required.").WithErrorCode("Student.Address.City.Empty");

        RuleFor(s => s.State).NotEmpty().WithMessage("State is required.").WithErrorCode("Student.Address.State.Empty");

        RuleFor(s => s.ZipCode).NotEmpty().WithMessage("Zip code is required.")
            .WithErrorCode("Student.Address.ZipCode.Empty");

        RuleFor(s => s.Country).NotEmpty().WithMessage("Country is required.")
            .WithErrorCode("Student.Address.Country.Empty");

        RuleFor(s => s.BuildingNumber).GreaterThan(0).WithMessage("Building number must be greater than zero.")
            .WithErrorCode("Student.Address.BuildingNumber.Invalid");
    }
}