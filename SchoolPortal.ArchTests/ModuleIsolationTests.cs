using NetArchTest.Rules;
using Xunit;

namespace SchoolPortal.ArchTests;

public class ModuleIsolationTests
{
    // Every ordered pair of distinct modules (Students->Grades, Grades->Students, ...).
    // Ordered, not combinations, because a violation could exist in only one direction.
    public static IEnumerable<object[]> ModulePairs =>
        from a in ModuleMap.All from b in ModuleMap.All where a.ModuleName != b.ModuleName select new object[] { a, b };

    [Theory]
    [MemberData(nameof(ModulePairs))]
    public void Module_should_not_depend_on_another_modules_layers(ModuleAssemblies module, ModuleAssemblies other)
    {
        // This is the test that would have caught the old IStudentDirectory
        // cross-module pattern before the move to MediatR events. If any type
        // in Students.* references Grades.Domain/Application/Infrastructure
        // (or vice versa), the modules aren't actually decoupled — they're just
        // in separate folders. Cross-module communication should only happen
        // through MediatR event contracts, never a direct type reference.
        var forbiddenNamespaces = new[]
        {
            other.Domain.GetName().Name!, other.Application.GetName().Name!, other.Infrastructure.GetName().Name!
        };

        var offenders = new[] { module.Domain, module.Application, module.Infrastructure }.SelectMany(assembly =>
            Types.InAssembly(assembly).ShouldNot().HaveDependencyOnAny(forbiddenNamespaces).GetResult().FailingTypes ??
            []).ToList();

        Assert.True(offenders.Count == 0,
            $"{module.ModuleName} depends directly on {other.ModuleName}. Offending types: " +
            string.Join(", ", offenders.Select(t => t.FullName)));
    }
}