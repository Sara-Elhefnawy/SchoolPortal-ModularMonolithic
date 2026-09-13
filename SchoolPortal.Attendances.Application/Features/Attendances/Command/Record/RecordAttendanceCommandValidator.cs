using FluentValidation;

namespace SchoolPortal.Attendances.Application.Features.Attendances.Command.Record;

public sealed class RecordAttendanceCommandValidator : AbstractValidator<RecordAttendanceCommand>
{
    public RecordAttendanceCommandValidator()
    {
        RuleFor(x => x.StudentId).GreaterThan(0).WithErrorCode("Attendance.InvalidStudentId")
            .WithMessage("A valid student id is required.");

        RuleFor(x => x.SessionOccurrenceId).GreaterThan(0).WithErrorCode("Attendance.InvalidSessionOccurrenceId")
            .WithMessage("A valid session occurrence id is required.");

        RuleFor(x => x.Status).IsInEnum().WithErrorCode("Attendance.InvalidStatus")
            .WithMessage("A valid attendance status is required.");

        RuleFor(x => x.Note).MaximumLength(250).WithErrorCode("Attendance.InvalidNote")
            .WithMessage("Note must be 250 characters or fewer.");
    }
}