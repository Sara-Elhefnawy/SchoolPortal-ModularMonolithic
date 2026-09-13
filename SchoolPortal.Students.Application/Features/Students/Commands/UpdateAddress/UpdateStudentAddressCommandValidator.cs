using FluentValidation;

namespace SchoolPortal.Students.Application.Features.Students.Commands.UpdateAddress;

public sealed class UpdateStudentAddressCommandValidator : AbstractValidator<UpdateStudentAddressCommand>
{
    public UpdateStudentAddressCommandValidator()
    {
        RuleFor(s => s.StudentId).GreaterThan(0).WithMessage("A valid student id is required.")
            .WithErrorCode("Student.Id.Invalid");
        
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