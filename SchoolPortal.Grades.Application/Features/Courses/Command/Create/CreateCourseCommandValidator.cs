using FluentValidation;

namespace SchoolPortal.Grades.Application.Features.Courses.Command.Create;

public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.CreditHours).GreaterThan(0);
    }
}