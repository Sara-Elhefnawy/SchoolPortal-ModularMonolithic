using FluentValidation;

namespace SchoolPortal.Grades.Application.Features.AcademicYears.Command.Create;

public sealed class CreateAcademicYearCommandValidator : AbstractValidator<CreateAcademicYearCommand>
{
    public CreateAcademicYearCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.StartDate).NotEqual(default(DateOnly));
        RuleFor(x => x.EndDate).NotEqual(default(DateOnly));
    }
}