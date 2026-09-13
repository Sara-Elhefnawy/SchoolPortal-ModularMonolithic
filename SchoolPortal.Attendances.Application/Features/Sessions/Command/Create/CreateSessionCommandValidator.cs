using FluentValidation;

namespace SchoolPortal.Attendances.Application.Features.Sessions.Command.Create;

public sealed class CreateSessionCommandValidator : AbstractValidator<CreateSessionCommand>
{
    public CreateSessionCommandValidator()
    {
        RuleFor(s => s.CourseSectionId).GreaterThan(0).WithErrorCode("Session.InvalidCourseSectionId")
            .WithMessage("A valid course section id is required.");

        RuleFor(s => s.DayOfWeek).IsInEnum().WithErrorCode("Session.InvalidDayOfWeek")
            .WithMessage("A valid day of week is required.");

        RuleFor(s => s.StartTime).NotEqual(default(TimeOnly)).WithErrorCode("Session.InvalidStartTime")
            .WithMessage("A valid start time is required.");

        RuleFor(s => s).Must(s => s.StartTime < s.EndTime).WithErrorCode("Session.InvalidTimeRange")
            .WithMessage("StartTime must be before EndTime.");
    }
}