using FluentValidation;

namespace SchoolPortal.Students.Application.Features.Enrollments.Commands.Withdraw;

public sealed class WithdrawEnrollmentCommandValidator : AbstractValidator<WithdrawEnrollmentCommand>
{
    public WithdrawEnrollmentCommandValidator()
    {
        RuleFor(e => e.EnrollmentId).GreaterThan(0).WithMessage("A valid enrollment id is required.")
            .WithErrorCode("Enrollment.Id.Invalid");
    }
}