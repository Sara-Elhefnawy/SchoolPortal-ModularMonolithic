using FluentValidation;
using SchoolPortal.Students.Application.Features.Enrollments.Commands.Withdraw;

namespace SchoolPortal.Students.Application.Features.Enrollments.Commands.Complete;

public sealed class CompleteEnrollmentCommandValidator : AbstractValidator<WithdrawEnrollmentCommand>
{
    public CompleteEnrollmentCommandValidator()
    {
        RuleFor(e => e.EnrollmentId).GreaterThan(0).WithMessage("A valid enrollment id is required.")
            .WithErrorCode("Enrollment.Id.Invalid");
    }
}