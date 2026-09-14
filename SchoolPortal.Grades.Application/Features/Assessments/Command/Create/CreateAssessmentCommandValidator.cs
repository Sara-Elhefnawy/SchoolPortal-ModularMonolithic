using FluentValidation;

namespace SchoolPortal.Grades.Application.Features.Assessments.Command.Create;

public sealed class CreateAssessmentCommandValidator : AbstractValidator<CreateAssessmentCommand>
{
    public CreateAssessmentCommandValidator()
    {
        RuleFor(x => x.CourseSectionId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.MaximumScore).GreaterThan(0);
        RuleFor(x => x.Weight).GreaterThan(0).LessThanOrEqualTo(1);
        RuleFor(x => x.Date).NotEqual(default(DateOnly));
    }
}