using FluentValidation;

namespace SchoolPortal.Students.Application.Features.Enrollments.Commands.Enroll;

public sealed class EnrollStudentCommandValidator : AbstractValidator<EnrollStudentCommand>
{
    public EnrollStudentCommandValidator()
    {
        RuleFor(e => e.StudentId).GreaterThan(0).WithMessage("A valid student id is required.")
            .WithErrorCode("Enrollment.StudentId.Invalid");

        RuleFor(e => e.CourseSectionId).GreaterThan(0).WithMessage("A valid course section id is required.")
            .WithErrorCode("Enrollment.CourseSectionId.Invalid");

        RuleFor(e => e.Date).NotEqual(default(DateOnly)).WithMessage("Enrollment date is required.")
            .WithErrorCode("Enrollment.Date.Empty");
    }
}