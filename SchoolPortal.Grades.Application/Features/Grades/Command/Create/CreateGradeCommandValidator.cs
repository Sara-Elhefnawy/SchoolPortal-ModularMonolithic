using FluentValidation;

namespace SchoolPortal.Grades.Application.Features.Grades.Command.Create;

public sealed class CreateGradeCommandValidator : AbstractValidator<CreateGradeCommand>
{
    public CreateGradeCommandValidator()
    {
        RuleFor(x => x.StudentId).GreaterThan(0);
        RuleFor(x => x.AssessmentId).GreaterThan(0);
        RuleFor(x => x.Score).GreaterThanOrEqualTo(0);
        // NOT validating Score against MaximumScore here — that requires
        // looking up the Assessment, which is runtime state a validator
        // shouldn't reach for. The handler does that check via Grade.Create.
    }
}