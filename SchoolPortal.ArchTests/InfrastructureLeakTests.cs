using NetArchTest.Rules;
using Xunit;

namespace SchoolPortal.ArchTests;

public class InfrastructureLeakTests
{
    [Fact]
    public void Only_grades_infrastructure_should_use_dapper()
    {
        // This isn't a generic clean-architecture rule — it's your specific
        // decision (EF Core for writes everywhere, Dapper only for Grades'
        // aggregation-heavy reporting/ranking queries). Writing it as a test
        // means "why is there a SqlConnection in Attendance.Infrastructure?"
        // gets caught in CI instead of in a code review six months from now.
        foreach (var module in ModuleMap.All.Where(m => m.ModuleName != "Grades"))
        {
            var result = Types.InAssembly(module.Infrastructure)
                .ShouldNot()
                .HaveDependencyOn("Dapper")
                .GetResult();

            Assert.True(result.IsSuccessful,
                $"{module.ModuleName}.Infrastructure depends on Dapper, but Dapper is reserved " +
                $"for Grades' reporting/ranking queries. Offending types: " +
                string.Join(", ", result.FailingTypes?.Select(t => t.Name) ?? []));
        }
    }
}