using FluentValidation;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Command.Update;

public sealed class UpdateAttendanceStatusCommandValidator : AbstractValidator<UpdateAttendanceStatusCommand>
{
    public UpdateAttendanceStatusCommandValidator()
    {
        RuleFor(x => x.AttendanceId).GreaterThan(0).WithErrorCode("Attendance.InvalidAttendanceId")
            .WithMessage("A valid attendance id is required.");

        RuleFor(x => x.Status).IsInEnum().WithErrorCode("Attendance.InvalidStatus")
            .WithMessage("A valid attendance status is required.");

        RuleFor(x => x.Note).MaximumLength(250).WithErrorCode("Attendance.InvalidNote")
            .WithMessage("Note must be 250 characters or fewer.");
    }
}