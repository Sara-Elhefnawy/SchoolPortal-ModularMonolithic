using SchoolPortal.Grades.Application.Directories;
using SchoolPortal.Grades.Application.Persistence;
using SchoolPortal.Grades.Domain.AcademicYears;
using SchoolPortal.Grades.Domain.Assessments;
using SchoolPortal.Grades.Domain.CourseSections;
using SchoolPortal.Grades.Domain.Courses;
using SchoolPortal.Grades.Domain.Grades;
using SchoolPortal.Grades.Domain.Terms;
using SchoolPortal.SharedKernel.Persistence.Seeders;

namespace SchoolPortal.Grades.Infrastructure.Persistence.Seeders;

internal sealed class GradesSeeder(
    IAcademicYearDirectory academicYears,
    ITermDirectory terms,
    ICourseDirectory courses,
    ICourseSectionDirectory courseSections,
    IAssessmentDirectory assessments,
    IGradeDirectory grades,
    IGradesUnitOfWork unitOfWork,
    TimeProvider clock) : IDataSeeder
{
    public int Order => 40;

    public async Task SeedAsync(CancellationToken ct)
    {
        // WHY the root table as the guard: the seeder runs on every startup, and the
        // unique indexes (year name, course code, etc.) would throw on a second run.
        if ((await academicYears.GetAllAsync(ct)).Count > 0)
            return;

        var today = DateOnly.FromDateTime(clock.GetLocalNow().DateTime);

        // WHY computed: a hard-coded "2026/2027" silently becomes the wrong year later.
        // Egypt's school year starts around September.
        var startYear = today.Month >= 8 ? today.Year : today.Year - 1;

        // ---- Academic year (only ONE may be current: filtered unique index on IsCurrent) ----
        var year = AcademicYear.Create(
            $"{startYear}/{startYear + 1}", new DateOnly(startYear, 9, 1), new DateOnly(startYear + 1, 6, 30)).Unwrap("academic year");

        academicYears.Add(year);
        await unitOfWork.SaveChangesAsync(ct); // year.Id is needed by the terms

        // ---- Terms ----
        var firstTerm = Term.Create("FirstTerm", TermType.FirstTerm, new DateOnly(startYear, 9, 1),
            new DateOnly(startYear + 1, 1, 31), year.Id).Unwrap("First term");

        var secondTerm = Term.Create("SecondTerm", TermType.SecondTerm, new DateOnly(startYear + 1, 2, 1),
            new DateOnly(startYear + 1, 6, 30), year.Id).Unwrap("Second term");

        var final = Term.Create("Final", TermType.Final, new DateOnly(startYear + 1, 7, 1),
            new DateOnly(startYear + 1, 7, 30), year.Id).Unwrap("Final term");

        terms.Add(firstTerm);
        terms.Add(secondTerm);
        terms.Add(final);

        // ---- Courses ----
        var courseList = new (string Name, string Code, decimal CreditHours)[]
        {
            ("Introduction to Programming", "CS101", 3m), ("Calculus I", "MATH101", 4m),
            ("Academic Writing", "ENG101", 2m)
        }.Select(c => Course.Create(c.Name, c.Code, c.CreditHours).Unwrap(c.Code)).ToList();

        foreach (var course in courseList)
            courses.Add(course);

        // Terms and courses don't depend on each other, so one save covers both.
        await unitOfWork.SaveChangesAsync(ct);

        // ---- Course sections (course + fall term) ----
        // WHY one per course, in course order: that makes section IDs 1..3, which is
        // what SeedConstants.FullyEnrolledSectionIds relies on.
        var sections = courseList.Select(c => CourseSection.Create(c.Id, firstTerm.Id).Unwrap($"section for {c.Code}"))
            .ToList();

        foreach (var section in sections)
            courseSections.Add(section);

        await unitOfWork.SaveChangesAsync(ct);

        // ---- Assessments (only for the "fully enrolled" sections) ----
        // Weights are fractions (column is numeric(4,3)); the rankings query divides
        // by the total weight of graded assessments, so they needn't sum to 1.
        var plan = new (string Name, AssessmentType Type, decimal Max, decimal Weight, int DaysAgo)[]
        {
            ("Quiz 1", AssessmentType.Quiz, 20m, 0.2m, 21), ("Midterm", AssessmentType.Midterm, 100m, 0.3m, 10),
            // WHY in the future: a final exam that already happened mid-term is unrealistic,
            // and it gives the UI an assessment with no grades yet.
            ("FinalExam", AssessmentType.FinalExam, 100m, 0.5m, -90)
        };

        var assessmentList = new List<Assessment>();

        foreach (var section in sections.Where(s => SeedConstants.FullyEnrolledSectionIds.Contains(s.Id)))
        foreach (var p in plan)
        {
            assessmentList.Add(Assessment.Create(section.Id, p.Name, p.Type, p.Max, p.Weight, today.AddDays(-p.DaysAgo))
                .Unwrap($"assessment {p.Name}"));
        }

        foreach (var assessment in assessmentList)
            assessments.Add(assessment);

        await unitOfWork.SaveChangesAsync(ct);

        // ---- Grades (only for assessments that already happened) ----
        // WHY fixed seed: same "random" scores on every fresh database.
        var rng = new Random(42);

        foreach (var assessment in assessmentList.Where(a => a.Date <= today))
        foreach (var studentId in SeedConstants.StudentIds)
        {
            // 50%-100% of the max score, so nobody is at 0 and rankings have spread.
            var score = Math.Round(assessment.MaximumScore * (decimal)(0.5 + rng.NextDouble() * 0.5), 1);

            var grade = Grade.Create(studentId, assessment.Id, score, 100).Unwrap("grade");

            // WHY: rankings only count Published/Locked grades, and a new grade is a draft.
            grade.Submit().EnsureSuccess("grade submit");
            grade.Publish().EnsureSuccess("grade publish");

            grades.Add(grade);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}