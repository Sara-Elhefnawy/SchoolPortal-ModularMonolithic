using FluentValidation;

namespace SchoolPortal.Students.Application.Features.Students.Commands.UpdateEmail;

public sealed class UpdateStudentEmailCommandValidator : AbstractValidator<UpdateStudentEmailCommand>
{
    public UpdateStudentEmailCommandValidator()
    {
        RuleFor(s => s.StudentId).GreaterThan(0).WithMessage("A valid student id is required.")
            .WithErrorCode("Student.Id.Invalid");
        
        RuleFor(s => s.Email).NotEmpty().EmailAddress().WithMessage("A valid email address is required.")
            .WithErrorCode("Student.Email.Invalid");
    }
}