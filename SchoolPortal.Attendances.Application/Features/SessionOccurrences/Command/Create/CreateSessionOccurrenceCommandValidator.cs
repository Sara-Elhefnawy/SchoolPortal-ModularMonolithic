using FluentValidation;

namespace SchoolPortal.Attendances.Application.Features.SessionOccurrences.Command.Create;

public sealed class CreateSessionOccurrenceCommandValidator : AbstractValidator<CreateSessionOccurrenceCommand>
{
    public CreateSessionOccurrenceCommandValidator()
    {
        RuleFor(x => x.SessionId).GreaterThan(0).WithErrorCode("SessionOccurrence.InvalidSessionId")
            .WithMessage("A valid session id is required.");

        RuleFor(x => x.Date).NotEqual(default(DateOnly)).WithErrorCode("SessionOccurrence.InvalidDate")
            .WithMessage("A valid date is required.");
    }
}