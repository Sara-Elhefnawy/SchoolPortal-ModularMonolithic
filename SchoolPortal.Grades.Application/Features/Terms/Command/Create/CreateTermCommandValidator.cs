using FluentValidation;

namespace SchoolPortal.Grades.Application.Features.Terms.Command.Create;

public sealed class CreateTermCommandValidator : AbstractValidator<CreateTermCommand>
{
    public CreateTermCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.StartDate).NotEqual(default(DateOnly));
        RuleFor(x => x.EndDate).NotEqual(default(DateOnly));
        RuleFor(x => x.AcademicYearId).GreaterThan(0);
    }
}