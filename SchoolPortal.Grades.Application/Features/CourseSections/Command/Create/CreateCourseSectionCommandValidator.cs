using FluentValidation;

namespace SchoolPortal.Grades.Application.Features.CourseSections.Command.Create;

public sealed class CreateCourseSectionCommandValidator : AbstractValidator<CreateCourseSectionCommand>
{
    public CreateCourseSectionCommandValidator()
    {
        RuleFor(x => x.CourseId).GreaterThan(0);
        RuleFor(x => x.TermId).GreaterThan(0);
    }
}