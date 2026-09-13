using FluentValidation;

namespace SchoolPortal.Students.Application.Features.Students.Commands.UpdatePhone;

public sealed class UpdateStudentPhoneCommandValidator : AbstractValidator<UpdateStudentPhoneCommand>
{
    public UpdateStudentPhoneCommandValidator()
    {
        RuleFor(s => s.StudentId).GreaterThan(0).WithMessage("A valid student id is required.")
            .WithErrorCode("Student.Id.Invalid");
        
        RuleFor(s => s.Phone).NotEmpty().WithMessage("Phone number is required.")
            .WithErrorCode("Student.PhoneNumber.Empty");
    }
}